//******************************************************************************************************
//  BenRecord.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/06/2016 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenDownloader
{
    public class BenRecord
    {
        #region [Members]
        private int m_id;
        private DateTime m_dateTime;
        private int m_size;
        #endregion

        #region [Constructors]
        public BenRecord(int recordId, DateTime recordDateTime, int recordSize)
        {
            m_id = recordId;
            m_dateTime = recordDateTime;
            m_size = recordSize;
        }
        #endregion

        #region [Accessors]
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return m_dateTime;
            }
            set
            {
                m_dateTime = value;
            }
        }

        public int Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        #endregion
    }
}
