
using FluentNHibernate.Mapping;

namespace ZadanieRekrutacyjne.Models.Mapping;

public class TasksMapping : ClassMap<Tasks>
{
    TasksMapping()
    {
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Title).Length(255).Not.Nullable();
        Map(x => x.Description).Length(1000).Nullable();
        Map(x => x.Deadline).Nullable();
        Map(x => x.CreateTime).Nullable();
        Map(x => x.Progress).Nullable();
        Map(x => x.Tag).Nullable();
        Map(x => x.Details).Nullable();
        Table("Tasks");
    }
}
