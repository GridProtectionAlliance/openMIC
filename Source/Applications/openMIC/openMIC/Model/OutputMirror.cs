//******************************************************************************************************
//  OutputMirror.cs - Gbtc
//
//  Copyright © 2022, Grid Protection Alliance.  All Rights Reserved.
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
//  12/27/2022 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using GSF.TimeSeries.Adapters;
using Newtonsoft.Json;
using ConnectionStringParser = GSF.Configuration.ConnectionStringParser<GSF.TimeSeries.Adapters.ConnectionStringParameterAttribute>;

// ReSharper disable InconsistentNaming
namespace openMIC.Model;

public enum OutputMirrorConnectionType
{
    [Description("File System")]
    FileSystem,

    [Description("FTP")]
    FTP,

    [Description("SFTP")]
    SFTP,

    [Description("UNC - Windows Only")]
    UNC
}

// Defines output mirror connection string settings
public class OutputMirrorSettings
{
    [ConnectionStringParameter]
    [Description("Defines remote mirror root path.")]
    [DefaultValue("/")]
    public string RemotePath { get; set; }

    [ConnectionStringParameter]
    [Description("Defines remote mirror host name or IP.")]
    [DefaultValue("")]
    public string Host { get; set; }

    [ConnectionStringParameter]
    [Description("Defines remote mirror host port (if applicable).")]
    [DefaultValue(null)]
    public int? Port { get; set; }

    [ConnectionStringParameter]
    [Description("Defines remote mirror host connection username.")]
    [DefaultValue("")]
    public string Username { get; set; }

    [ConnectionStringParameter]
    [Description("Defines remote mirror host connection password (or pass-phrase).")]
    [DefaultValue("")]
    public string Password { get; set; }

    [ConnectionStringParameter]
    [Description("Defines remote mirror host key file (if applicable).")]
    [DefaultValue("")]
    public string KeyFile { get; set; }
}

[PrimaryLabel("Name")]
public class OutputMirror
{
    public OutputMirror()
    {
        Settings = new OutputMirrorSettings();
    }

    [PrimaryKey(true)]
    public int ID { get; set; }

    [Required]
    [StringLength(200)]
    [Label("Output Mirror Name")]
    public string Name { get; set; }

    [Required]
    [Label("Root Source Path to Mirror")]
    [DefaultValueExpression("Global.DefaultLocalPath")]
    public string Source { get; set; }

    [Required]
    [StringLength(200)]
    [Label("Mirror Connection Type")]
    [DefaultValue(nameof(OutputMirrorConnectionType.FileSystem))]
    public string ConnectionType
    {
        get => Type.ToString();
        set => Type = Enum.TryParse(value?.Trim(), true, out OutputMirrorConnectionType type) ? type : OutputMirrorConnectionType.FileSystem;
    }

    [NonRecordField]
    [JsonIgnore]
    public OutputMirrorConnectionType Type { get; set; } = OutputMirrorConnectionType.FileSystem;
    
    [FieldName("Settings")]
    public string ConnectionString
    {
        get
        {
            ConnectionStringParser parser = new();
            return parser.ComposeConnectionString(Settings);
        }
        set
        {
            ConnectionStringParser parser = new();
            parser.ParseConnectionString(value, Settings);
        }
    }

    [NonRecordField]
    [JsonIgnore]
    public OutputMirrorSettings Settings { get; internal set; }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy { get; set; }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy { get; set; }
}