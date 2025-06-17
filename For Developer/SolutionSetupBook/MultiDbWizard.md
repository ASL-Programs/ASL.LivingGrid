# MultiDbWizard — Auto-Discover və Manual DB Quraşdırılması

## Niyə yaradılıb?
Enterprise səviyyəli idarəetmə platformalarında müxtəlif verilənlər bazası növlərinin (SQL Server, PostgreSQL, SQLite və s.) avtomatik aşkar edilməsi, əl ilə əlavə olunması və ilkin quraşdırılması üçün istifadəçiyə rahat və təhlükəsiz interfeys təqdim etmək məqsədilə yaradılıb.

## Nəyə xidmət edir?
- Şəbəkədə mövcud olan verilənlər bazası serverlərini və instansiyalarını avtomatik aşkar edir.
- İstifadəçiyə manual connection string daxil etməyə imkan verir.
- Qoşulmanı test edir və uğurlu olduqda ilkin migrate/install əməliyyatını avtomatik icra edir.
- Bütün əməliyyatlar UI-dan idarə olunur, kod dəyişməsinə ehtiyac yoxdur.

## İstifadə qaydası və idarəetmə prinsipləri
1. "Multi-Database Wizard" səhifəsinə keçin.
2. "Database Type" seçin (məs: SQLServer).
3. "Discover" düyməsi ilə şəbəkədəki mövcud verilənlər bazalarını tapın və siyahıdan seçin və ya connection string-i manual daxil edin.
4. "Test Connection" düyməsi ilə qoşulmanı yoxlayın.
5. "Apply" düyməsi ilə konfiqurasiyanı tətbiq edin və ilkin migrate/install baş versin.

## Texniki və biznes üstünlükləri
- İstənilən DB növü və instansiyası üçün çevik və təhlükəsiz quraşdırma.
- Manual və avtomatik aşkar etmə imkanı.
- UI-dan idarəetmə, kod dəyişmədən konfiqurasiya.
- Enterprise səviyyədə audit və təhlükəsizlik.

## Gələcək inkişaf yolları və risklər
- Şəbəkədə daha geniş və ağıllı DB discovery (LDAP, mDNS, port scan və s.)
- Daha çox DB növü və cloud DB-lər üçün dəstək.
- Qoşulma və migrate prosesinin daha detallı loglanması və səhv hallarda avtomatik rollback.
- İstifadəçi üçün daha ətraflı kömək və troubleshooting səhifələri.

## İstifadəçi və developer üçün hər bir detal
- Hər bir dəyişiklik və əməliyyat audit log-da qeyd olunur.
- Bütün connection string-lər və konfiqurasiya dəyişiklikləri şifrələnir və yalnız icazəli istifadəçilər üçün görünür.
- UI-da hər addımda status və nəticə göstərilir.

---
Sənəd tam Azərbaycan dilində hazırlanıb, yalnız terminlər ingilis dilində saxlanılıb.
