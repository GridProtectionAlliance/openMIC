using System.Data.Entity;

namespace openMIC.Model
{
    public class openMICData : DbContext
    {
        public openMICData()
            : base("name=openMICData")
        {
        }

        public virtual DbSet<AccessLog> AccessLogs
        {
            get;
            set;
        }

        public virtual DbSet<Alarm> Alarms
        {
            get;
            set;
        }

        public virtual DbSet<AlarmLog> AlarmLogs
        {
            get;
            set;
        }

        public virtual DbSet<ApplicationRole> ApplicationRoles
        {
            get;
            set;
        }

        public virtual DbSet<Company> Companies
        {
            get;
            set;
        }

        public virtual DbSet<CustomActionAdapter> CustomActionAdapters
        {
            get;
            set;
        }

        public virtual DbSet<CustomInputAdapter> CustomInputAdapters
        {
            get;
            set;
        }

        public virtual DbSet<CustomOutputAdapter> CustomOutputAdapters
        {
            get;
            set;
        }

        public virtual DbSet<Device> Devices
        {
            get;
            set;
        }

        public virtual DbSet<ErrorLog> ErrorLogs
        {
            get;
            set;
        }

        public virtual DbSet<Historian> Historians
        {
            get;
            set;
        }

        public virtual DbSet<Interconnection> Interconnections
        {
            get;
            set;
        }

        public virtual DbSet<Measurement> Measurements
        {
            get;
            set;
        }

        public virtual DbSet<Node> Nodes
        {
            get;
            set;
        }

        public virtual DbSet<Protocol> Protocols
        {
            get;
            set;
        }

        public virtual DbSet<SecurityGroup> SecurityGroups
        {
            get;
            set;
        }

        public virtual DbSet<SignalType> SignalTypes
        {
            get;
            set;
        }

        public virtual DbSet<Statistic> Statistics
        {
            get;
            set;
        }

        public virtual DbSet<UserAccount> UserAccounts
        {
            get;
            set;
        }

        public virtual DbSet<Vendor> Vendors
        {
            get;
            set;
        }

        public virtual DbSet<VendorDevice> VendorDevices
        {
            get;
            set;
        }

        public virtual DbSet<DataOperation> DataOperations
        {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessLog>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Alarm>()
                .Property(e => e.TagName)
                .IsUnicode(false);

            modelBuilder.Entity<Alarm>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Alarm>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Alarm>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ApplicationRole>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ApplicationRole>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ApplicationRole>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ApplicationRole>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.MapAcronym)
                .IsFixedLength();

            modelBuilder.Entity<Company>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.URL)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.AdapterName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.ConnectionString)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomActionAdapter>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.AdapterName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.ConnectionString)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomInputAdapter>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.AdapterName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.ConnectionString)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CustomOutputAdapter>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.OriginalSource)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.Longitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Device>()
                .Property(e => e.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Device>()
                .Property(e => e.ConnectionString)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.TimeZone)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Detail)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.ConnectionString)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Historian>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Interconnection>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Interconnection>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.PointTag)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.AlternateTag)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.SignalReference)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Measurement>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Longitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Node>()
                .Property(e => e.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Node>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.ImagePath)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Settings)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.MenuType)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.MenuData)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<Protocol>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityGroup>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<SecurityGroup>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.Suffix)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.Abbreviation)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.LongAcronym)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<SignalType>()
                .Property(e => e.EngineeringUnits)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.MethodName)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.Arguments)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.DataType)
                .IsUnicode(false);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.DisplayFormat)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserAccount>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Acronym)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.ContactEmail)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.URL)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Vendor>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<VendorDevice>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<VendorDevice>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<VendorDevice>()
                .Property(e => e.URL)
                .IsUnicode(false);

            modelBuilder.Entity<VendorDevice>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<VendorDevice>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<DataOperation>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DataOperation>()
                .Property(e => e.AssemblyName)
                .IsUnicode(false);

            modelBuilder.Entity<DataOperation>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<DataOperation>()
                .Property(e => e.MethodName)
                .IsUnicode(false);

            modelBuilder.Entity<DataOperation>()
                .Property(e => e.Arguments)
                .IsUnicode(false);
        }
    }
}