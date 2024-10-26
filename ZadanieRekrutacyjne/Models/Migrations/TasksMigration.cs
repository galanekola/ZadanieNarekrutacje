namespace ZadanieRekrutacyjne.Models.Migrations;
using FluentMigrator;
[Migration(001)]
public class CreateTableTasks : Migration
{
    public override void Up()
    {
        Create.Table("Tasks")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("Progress").AsInt32().Nullable()
            .WithColumn("Tag").AsString().Nullable()
            .WithColumn("Deadline").AsDateTime2().Nullable()
            .WithColumn("CreateTime").AsDateTime2().Nullable()
            .WithColumn("Details").AsString().Nullable();
    }
    public override void Down()
    {
        Delete.ForeignKey("FK_Tasks_Tabela").OnTable("Tasks");
        Delete.Table("Tasks");
    }
}
