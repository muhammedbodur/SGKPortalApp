import re
import codecs

def parse_birim():
    """Birim (Departman) verilerini parse et"""
    with codecs.open(r'F:\SQL Data\birim.sql', 'r', encoding='utf-8') as f:
        content = f.read()
    
    # INSERT satırlarını bul
    pattern = r"INSERT INTO `birim` VALUES \((\d+), '([^']+)', '([^']+)'"
    matches = re.findall(pattern, content)
    
    sql_lines = []
    sql_lines.append("-- Departmanlar (birim)")
    sql_lines.append("SET IDENTITY_INSERT PER_Departmanlar ON;")
    sql_lines.append("GO\n")
    
    for kod, birimad, kisaad in matches:
        birimad_clean = birimad.replace("'", "''")
        kisaad_clean = kisaad.replace("'", "''")
        sql_lines.append(f"IF NOT EXISTS (SELECT 1 FROM PER_Departmanlar WHERE DepartmanId = {kod})")
        sql_lines.append(f"    INSERT INTO PER_Departmanlar (DepartmanId, DepartmanAdi, DepartmanAdiKisa, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)")
        sql_lines.append(f"    VALUES ({kod}, N'{birimad_clean}', N'{kisaad_clean}', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());")
        sql_lines.append("")
    
    sql_lines.append("SET IDENTITY_INSERT PER_Departmanlar OFF;")
    sql_lines.append("GO\n")
    sql_lines.append(f"PRINT '✅ {len(matches)} Departman import edildi';")
    sql_lines.append("GO\n")
    
    return "\n".join(sql_lines)

def parse_servisler():
    """Servis verilerini parse et"""
    with codecs.open(r'F:\SQL Data\servisler.sql', 'r', encoding='utf-8') as f:
        content = f.read()
    
    # INSERT satırlarını bul
    pattern = r"INSERT INTO `servisler` VALUES \((\d+), '([^']+)'"
    matches = re.findall(pattern, content)
    
    sql_lines = []
    sql_lines.append("-- Servisler")
    sql_lines.append("SET IDENTITY_INSERT PER_Servisler ON;")
    sql_lines.append("GO\n")
    
    for servisid, servisadi in matches:
        servisadi_clean = servisadi.replace("'", "''").replace("\\n", "").strip()
        sql_lines.append(f"IF NOT EXISTS (SELECT 1 FROM PER_Servisler WHERE ServisId = {servisid})")
        sql_lines.append(f"    INSERT INTO PER_Servisler (ServisId, ServisAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)")
        sql_lines.append(f"    VALUES ({servisid}, N'{servisadi_clean}', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());")
        sql_lines.append("")
    
    sql_lines.append("SET IDENTITY_INSERT PER_Servisler OFF;")
    sql_lines.append("GO\n")
    sql_lines.append(f"PRINT '✅ {len(matches)} Servis import edildi';")
    sql_lines.append("GO\n")
    
    return "\n".join(sql_lines)

def parse_unvanlar():
    """Ünvan verilerini parse et"""
    with codecs.open(r'F:\SQL Data\unvanlar.sql', 'r', encoding='utf-8') as f:
        content = f.read()
    
    # INSERT satırlarını bul
    pattern = r"INSERT INTO `unvanlar` VALUES \('([^']+)', (\d+)\)"
    matches = re.findall(pattern, content)
    
    sql_lines = []
    sql_lines.append("-- Ünvanlar")
    sql_lines.append("SET IDENTITY_INSERT PER_Unvanlar ON;")
    sql_lines.append("GO\n")
    
    for unvan, unvan_id in matches:
        unvan_clean = unvan.replace("'", "''")
        sql_lines.append(f"IF NOT EXISTS (SELECT 1 FROM PER_Unvanlar WHERE UnvanId = {unvan_id})")
        sql_lines.append(f"    INSERT INTO PER_Unvanlar (UnvanId, UnvanAdi, Aktiflik, EkleyenKullanici, EklenmeTarihi, DuzenleyenKullanici, DuzenlenmeTarihi)")
        sql_lines.append(f"    VALUES ({unvan_id}, N'{unvan_clean}', 0, 'IMPORT', GETDATE(), 'IMPORT', GETDATE());")
        sql_lines.append("")
    
    sql_lines.append("SET IDENTITY_INSERT PER_Unvanlar OFF;")
    sql_lines.append("GO\n")
    sql_lines.append(f"PRINT '✅ {len(matches)} Ünvan import edildi';")
    sql_lines.append("GO\n")
    
    return "\n".join(sql_lines)

# Ana script
if __name__ == "__main__":
    print("Legacy data parse ediliyor...")
    
    output = []
    output.append("-- ============================================================================")
    output.append("-- SGK PORTAL - LEGACY DATA IMPORT SCRIPT")
    output.append("-- MySQL dump dosyalarından dönüştürülmüş SQL Server seed data")
    output.append("-- ============================================================================")
    output.append("-- NOT: Kullanıcı verileri bu script'e DAHİL DEĞİLDİR")
    output.append("-- Sadece Departman, Servis ve Ünvan verileri import edilir")
    output.append("-- ============================================================================\n")
    output.append("USE SGKPortalDB;")
    output.append("GO\n")
    output.append("PRINT '╔════════════════════════════════════════════════════════╗';")
    output.append("PRINT '║     LEGACY DATA IMPORT BAŞLIYOR...                    ║';")
    output.append("PRINT '╚════════════════════════════════════════════════════════╝';")
    output.append("PRINT '';")
    output.append("GO\n")
    
    # Departmanlar
    output.append("-- ============================================================================")
    output.append("-- 1. DEPARTMANLAR")
    output.append("-- ============================================================================")
    output.append(parse_birim())
    output.append("")
    
    # Servisler
    output.append("-- ============================================================================")
    output.append("-- 2. SERVİSLER")
    output.append("-- ============================================================================")
    output.append(parse_servisler())
    output.append("")
    
    # Ünvanlar
    output.append("-- ============================================================================")
    output.append("-- 3. ÜNVANLAR")
    output.append("-- ============================================================================")
    output.append(parse_unvanlar())
    output.append("")
    
    # Özet
    output.append("-- ============================================================================")
    output.append("-- ÖZET")
    output.append("-- ============================================================================")
    output.append("PRINT '';")
    output.append("PRINT '╔════════════════════════════════════════════════════════╗';")
    output.append("PRINT '║     LEGACY DATA IMPORT TAMAMLANDI!                    ║';")
    output.append("PRINT '╚════════════════════════════════════════════════════════╝';")
    output.append("PRINT '';")
    output.append("PRINT '📊 Import Edilen Veriler:';")
    output.append("PRINT '  • Departmanlar (birim)';")
    output.append("PRINT '  • Servisler';")
    output.append("PRINT '  • Ünvanlar';")
    output.append("PRINT '';")
    output.append("PRINT '⚠️  NOT: Kullanıcı verileri import EDİLMEDİ (ID çakışması olmaması için)';")
    output.append("PRINT '';")
    output.append("GO")
    
    # Dosyaya yaz
    with codecs.open(r'd:\AspNetExamples\SGKPortalApp\IMPORT_DATA.sql', 'w', encoding='utf-8') as f:
        f.write("\n".join(output))
    
    print("✅ IMPORT_DATA.sql dosyası oluşturuldu!")
