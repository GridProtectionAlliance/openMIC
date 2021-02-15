//******************************************************************************************************
//  Analyzer.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/21/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DranetzDowloader
{
    /// <summary>
    /// Dranetz metere recording session. Implements ICompare on Timestamp for sorting
    /// </summary>
    public class Session: IComparable
    {
        #region [ Fields ]
        private List<Record>? m_record = null;
        private DranetzConnection? m_connection = null;

        #endregion

        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int NumberofRecords {get; set; }

        public void SetConnection(DranetzConnection connection)
        {
            m_connection = connection;
        }
        public List<Record> Records
        {
            get { return m_record ?? (m_record = GetRecords().Result); }
        }

        private async Task<List<Record>> GetRecords(DateTime? Tstart = null, DateTime? Tend = null, bool completeEvent = true)
        {
            

            if (NumberofRecords > 0 && m_connection != null)
            {
                // For some Reason the Meter only sends up to 32 Records at a time??
                // It also lock out after a certain number of connections -> every 30 requests we add a pause
                
                // If no end date is specified we start at the top and work our way backwards

                int start = NumberofRecords - 32;
                int end = NumberofRecords - 1;
                byte[] data;
                List<Record> result = new List<Record>();

                // If an end date is specified we instead start with a record lower than this
                if (Tend != null)
                {
                    end = await FindRecord((DateTime)Tend);
                    start = end - 31;
                }

                int setEnd = end;
                int setStart = end;
                while (start > 0) 
                {
                    Console.WriteLine($"Polling Records ({start}) - ({end})...");
                    data = await  m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={start}-{end}");
                    end = start - 1;
                    setStart = start;
                    start = end - 31;
                    result.AddRange(Record.Parse(data));

                    if (Tstart != null && result[result.Count - 30].Time < Tstart)
                        break;
                }

                if (start < 0)
                {
                    Console.WriteLine($"Polling Records ({0}) - ({end})...");
                    data = m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={0}-{end}").Result;
                    setStart = 0;
                    result.AddRange(Record.Parse(data));
                }
                
                // If there is a Set of Event Records we need to make sure we get all of them.
                // Only do this for +- 93 Records at a time -> this will limit it to 3 downloads without eventdata before stopping
                // only relevant if there are any eventRecords in this Session
                if (completeEvent && result.Any(item => item.Type == (int)RecordType.HC_Wave))
                {
                    Console.WriteLine($"Polling additional EvendData Records to avoid partial PQDIF files...");
                    List<Record> eventData = new List<Record>();

                    int maxCycle = result.Where(item => item.Type == (int)RecordType.HC_Wave).Max(item => item.Cycle);
                    int minCycle = result.Where(item => item.Type == (int)RecordType.HC_Wave).Min(item => item.Cycle);

                    int sectionCycleMin;
                    int sectionCycleMax;
                    int Nfails = 0;
                    int evtSetEnd = end;
                    setStart = start;

                    // go backwards in Time first since most cases are pulling all available data 
                    if (setStart > 0)
                    {
                        while (setStart > 0 && Nfails < 4)
                        {
                            evtSetEnd = setStart - 1;
                            setStart = setStart - 32;
                            if (setStart < 0)
                                setStart = 0;
                            Console.WriteLine($"Polling Records ({setStart}) - ({evtSetEnd})...");
                            data = m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={setStart}-{evtSetEnd}").Result;
                            eventData = Record.Parse(data).Where(item => item.Type == (int)RecordType.HC_Wave).ToList();

                            if (eventData.Count == 0)
                            {
                                Nfails++;
                                continue;
                            }

                            sectionCycleMin = eventData.Min(item => item.Cycle);
                            sectionCycleMax = eventData.Max(item => item.Cycle);

                            if (Math.Abs(minCycle - sectionCycleMax) > 1)
                            {
                                Console.WriteLine($"Found Start of Event records...");
                                break;
                            }

                            if (eventData.Select(item => item.Cycle).Distinct().Count() < (sectionCycleMax - sectionCycleMin + 1) )
                            {
                                Console.WriteLine($"Found Start of Event records...");
                                result.AddRange(eventData);
                                break;
                            }
                            result.AddRange(eventData);
                            minCycle = sectionCycleMin;

                        }
                    }

                    // go forwards in time too in case historic data was specified
                    Nfails = 0;

                    
                    if (setEnd < NumberofRecords - 1)
                    {
                        while (setEnd < (NumberofRecords - 1) && Nfails < 4)
                        {
                            setStart = setEnd + 1;
                            setEnd = setEnd + 32;
                            
                            if (setEnd < (NumberofRecords - 1))
                                setEnd = (NumberofRecords - 1);

                            Console.WriteLine($"Polling Records ({setStart}) - ({setEnd})...");
                            data = m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={setStart}-{setEnd}").Result;
                            eventData = Record.Parse(data).Where(item => item.Type == (int)RecordType.HC_Wave).ToList();

                            if (eventData.Count == 0)
                            {
                                Nfails++;
                                continue;
                            }

                            sectionCycleMin = eventData.Min(item => item.Cycle);
                            sectionCycleMax = eventData.Max(item => item.Cycle);

                            if (Math.Abs(sectionCycleMin - maxCycle) > 1)
                            {
                                Console.WriteLine($"Found End of Event records...");
                                break;
                            }

                            if (eventData.Select(item => item.Cycle).Distinct().Count() < (sectionCycleMax - sectionCycleMin + 1))
                            {

                                // #ToDo Add logic to only add records up to event switch. That should fix up to two extra pqdiff files
                                Console.WriteLine($"Found End of Event records...");
                                result.AddRange(eventData);
                                break;
                            }
                            result.AddRange(eventData);
                            maxCycle = sectionCycleMax;

                        }
                    }
                }

                return result;
            }
            else
                return new List<Record>();

        }

        private async Task<int> FindRecord(DateTime TS)
        {

            // Start By getting last TS
            DateTime endTS = (await FindTSRecord(NumberofRecords - 1)).Time;
            DateTime startts = Time;
            // Find approx based on linear approx.
            
            int top = NumberofRecords - 1;
            int bottom = 0;
            int center = (int)Math.Floor(bottom + ((TS - startts).TotalMilliseconds / (endTS - startts).TotalMilliseconds) *(top - bottom));

            // Check ts at approx.
            while ((top - bottom) > 50)
            {
                DateTime centerTS = (await FindTSRecord(center)).Time;
                if (centerTS > TS)
                {
                    top = center;
                    endTS = centerTS;
                }
                if (centerTS <= TS)
                {
                    bottom = center;
                    startts = centerTS;
                }
                center = (int)Math.Floor(bottom + ((TS - startts).TotalMilliseconds / (endTS - startts).TotalMilliseconds) * (top - bottom));
                if (center == top || center == bottom)
                    center = (top + bottom) / 2;
            }
            return top;

        }

        private async Task<Record> FindTSRecord(int recordID)
        {
            byte[] data;
            Record result;
            int rec = recordID;

            data = await m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={rec}-{rec}");
            result = Record.Parse(data).First();

            while ( rec > 0 && result.Type == (int)RecordType.SYS_Event)
            {
                rec--;
                data = await m_connection.SendByteRequest($"getdbrecords&id=0&session_name={Name}&session_num={Id}&records={rec}-{rec}");
                result = Record.Parse(data).First();
                
            }

            return result;
            
        }
            
        public async Task<IEnumerable<Record>> LimitRecords(DateTime? Tstart = null, DateTime? Tend = null, bool completeEvent= true)
        {
            if (m_record != null && Tstart != null && Tend != null)
                return m_record.Where(item => item.Time > Tstart && item.Time < Tend);
            if (m_record != null && Tstart != null )
                return m_record.Where(item => item.Time > Tstart);
            if (m_record != null && Tend != null)
                return m_record.Where(item => item.Time < Tend);

            return await GetRecords(Tstart, Tend, completeEvent);
        }
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            Session? other = obj as Session;
            if (other != null)
                return this.Time.CompareTo(other.Time);
            else
                throw new ArgumentException("Object is not a Session");
        }

        #region [ static ] 



        #endregion
    }

}
