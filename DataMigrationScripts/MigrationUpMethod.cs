// Migration dosyasının Up() metoduna eklenecek kod
// Örnek: 20251103_RefactorUserPersonelRelationship.cs

protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Önce User tablosunu oluştur
    migrationBuilder.CreateTable(
        name: "CMN_Users",
        schema: "dbo",
        columns: table => new
        {
            TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
            PassWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            SessionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            AktifMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
            SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
            BasarisizGirisSayisi = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
            HesapKilitTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
            EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_CMN_Users", x => x.TcKimlikNo);
            table.ForeignKey(
                name: "FK_CMN_Users_PER_Personeller",
                column: x => x.TcKimlikNo,
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);
        });

    // 2. Mevcut Personeller için User kayıtları oluştur (DATA MIGRATION)
    migrationBuilder.Sql(@"
        INSERT INTO [dbo].[CMN_Users] 
        (
            TcKimlikNo, 
            PassWord, 
            SessionID,
            AktifMi, 
            SonGirisTarihi,
            BasarisizGirisSayisi, 
            HesapKilitTarihi,
            EklenmeTarihi, 
            DuzenlenmeTarihi, 
            SilindiMi,
            EkleyenKullanici,
            DuzenleyenKullanici
        )
        SELECT 
            p.TcKimlikNo,
            ISNULL(p.PassWord, p.TcKimlikNo) AS PassWord,  -- Eğer PassWord NULL ise TC Kimlik No kullan
            p.SessionID,
            CASE 
                WHEN p.PersonelAktiflikDurum = 1 THEN 1
                ELSE 0
            END AS AktifMi,
            NULL AS SonGirisTarihi,
            0 AS BasarisizGirisSayisi,
            NULL AS HesapKilitTarihi,
            p.EklenmeTarihi,
            p.DuzenlenmeTarihi,
            p.SilindiMi,
            p.EkleyenKullanici,
            p.DuzenleyenKullanici
        FROM [dbo].[PER_Personeller] p
        WHERE NOT EXISTS (
            SELECT 1 
            FROM [dbo].[CMN_Users] u 
            WHERE u.TcKimlikNo = p.TcKimlikNo
        );
        
        PRINT 'User kayıtları oluşturuldu: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
    ");

    // 3. Personel tablosundan PassWord ve SessionID kolonlarını kaldır
    migrationBuilder.DropColumn(
        name: "PassWord",
        schema: "dbo",
        table: "PER_Personeller");

    migrationBuilder.DropColumn(
        name: "SessionID",
        schema: "dbo",
        table: "PER_Personeller");

    // 4. Index'leri oluştur
    migrationBuilder.CreateIndex(
        name: "IX_CMN_Users_TcKimlikNo",
        schema: "dbo",
        table: "CMN_Users",
        column: "TcKimlikNo",
        unique: true,
        filter: "[SilindiMi] = 0");

    migrationBuilder.CreateIndex(
        name: "IX_CMN_Users_SessionID",
        schema: "dbo",
        table: "CMN_Users",
        column: "SessionID",
        filter: "[SessionID] IS NOT NULL AND [SilindiMi] = 0");

    // 5. HubConnection ilişkisini güncelle (eğer varsa)
    migrationBuilder.DropForeignKey(
        name: "FK_SIR_HubConnections_PER_Personeller",
        schema: "dbo",
        table: "SIR_HubConnections");

    migrationBuilder.AddForeignKey(
        name: "FK_SIR_HubConnections_CMN_Users",
        schema: "dbo",
        table: "SIR_HubConnections",
        column: "TcKimlikNo",
        principalSchema: "dbo",
        principalTable: "CMN_Users",
        principalColumn: "TcKimlikNo",
        onDelete: ReferentialAction.Cascade);
}
