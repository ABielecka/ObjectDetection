using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ObjectDetectionAn.Model.Mapping
{
    public class FrameObjectMap : EntityTypeConfiguration<FrameObject>
    {
        public FrameObjectMap()
        {
            HasKey(k => k.Id);
            Property(k => k.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(k => k.Person1).IsRequired().HasMaxLength(20);
            Property(k => k.Person2).IsRequired().HasMaxLength(20);
            Property(k => k.Distance).IsRequired();

            ToTable("FRAMES_DESCRIPTION");
            Property(k => k.Id).HasColumnName("ID");
            Property(k => k.Person1).HasColumnName("PERSON1");
            Property(k => k.Person2).HasColumnName("PERSON2");
            Property(k => k.Distance).HasColumnName("DISTANCE");
        }
    }
}
