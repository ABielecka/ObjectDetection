using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ObjectDetectionAn.Model.Mapping
{
    public class FrameMap : EntityTypeConfiguration<FrameModel>
    {
        public FrameMap()
        {
            HasKey(k => k.IdFrame);
            Property(k => k.IdFrame).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(k => k.Image).IsRequired();
            Property(k => k.Dtime).IsRequired();

            ToTable("FRAMES");
            Property(k => k.IdFrame).HasColumnName("ID");
            Property(k => k.Image).HasColumnName("FRAME");
            Property(k => k.Dtime).HasColumnName("DATE_TIME");

            HasMany(k => k.FrameObjects).WithOptional().Map(k => k.MapKey("ID_FRAME")).WillCascadeOnDelete(true);
        }
    }
}