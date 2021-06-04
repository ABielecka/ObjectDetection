using ObjectDetectionAn.Model.Mapping;
using Oracle.ManagedDataAccess.Client;
using System.Data.Entity;

namespace ObjectDetectionAn.Model
{
    public class Context : DbContext
    {
        static Context()
        {
            System.Data.Entity.Database.SetInitializer<Context>(null); //normalna praca
            //System.Data.Entity.Database.SetInitializer<Context>(new DropCreateDatabaseAlways<Context>()); //za kazdym razem usunie istniejace tabele z bazy danych i utworzy je od nowa
            //System.Data.Entity.Database.SetInitializer<Context>(new DropCreateDatabaseIfModelChanges<Context>()); //jeżeli zmieni się coś w modelach to wywali całą bazę danych i zalozy od nowa
        }

        public Context() : base(new OracleConnection(App._conString), false)
        {
        }

        public virtual DbSet<FrameModel> FrameModels { get; set; }
        public virtual DbSet<FrameObject> FrameObjects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("VA");

            modelBuilder.Configurations.Add(new FrameMap());
            modelBuilder.Configurations.Add(new FrameObjectMap());
        }
    }
}
