using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProMe.DataAccess.Migrations
{
    public partial class FullAddressFullTextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = "IF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[Contacts]'))\r\nALTER FULLTEXT INDEX ON [dbo].[Contacts] DISABLE\r\n\r\nGO\r\nIF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[Contacts]'))\r\nBEGIN\r\n\tDROP FULLTEXT INDEX ON [dbo].[Contacts]\r\nEnd\r\n\r\nGo\r\nIF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE [name]='FTCContacts')\r\nBEGIN\r\n\tDROP FULLTEXT CATALOG FTCContacts \r\nEND\r\n\r\nCREATE FULLTEXT CATALOG FTCContacts AS DEFAULT;\r\nCREATE FULLTEXT INDEX ON dbo.Contacts(Name) KEY INDEX PK_Contacts ON FTCContacts WITH STOPLIST = OFF, CHANGE_TRACKING AUTO;\r\n";
            migrationBuilder.Sql(sql, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sql = "DROP FULLTEXT INDEX on dbo.Addresses;\r\nDROP FULLTEXT CATALOG FTCAddress;";
            migrationBuilder.Sql(sql, true);
        }
    }
}
