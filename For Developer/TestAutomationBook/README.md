# TestAutomationBook

Bu sənəd WebAdminPanel modulunda test avtomatlaşdırma interfeysinin yaradılma məqsədini, istifadəsini və idarəetmə prinsiplərini izah edir. Sənəd tam Azərbaycan dilində saxlanılır, terminlər isə ingilis dilində göstərilir.

## Niyə yaradılıb?
- Testlərin UI-dan rahat icrası və nəticələrin izlənməsi üçün
- Avtomatik yeniləmə mexanizmi ilə sınaq mühitinin daim güncəllənməsi üçün

## Nəyə xidmət edir?
- `TestAutomation` səhifəsi vasitəsilə bütün unit, integration və e2e testləri çalışdırmaq
- Hər sınaqdan sonra nəticələri və logları tarixçə şəklində saxlamaq
- `tests.update` faylı aşkarlandıqda avtomatik testləri yenidən işlətmək

## İstifadə qaydası və idarəetmə prinsipləri
1. Menü vasitəsilə **Test Avtomatlaşdırması** bölməsini seçin
2. "Run All Tests" düyməsinə kliklədikdə `dotnet test --no-build` əmri arxa planda işləyir
3. "Check Updates" düyməsi `tests.update` markerinə əsasən yenilənmiş testləri avtomatik işə salır
4. Hər testin nəticəsi uğurlu və ya xətalı statusla birlikdə log şəklində saxlanılır

## Texniki və biznes üstünlükləri
- Testlərin asan icrası inkişaf prosesinin sürətini artırır
- UI-dan nəticələrin görüntülənməsi developer və QA komandaları üçün şəffaflıq yaradır
- Avtomatik yeniləmə mexanizmi test dəstlərinin həmişə aktual qalmasını təmin edir

## Gələcək inkişaf yolları və risklər
- Test nəticələrinin qrafik şəkildə analizi və bildiriş sistemi əlavə edilə bilər
- `dotnet test` əmri uzun çəkdiyi zaman server resurslarına təsir göstərə bilər; prioritetləşdirmə mexanizmi tələb oluna bilər

## İstifadəçi və developer üçün hər bir detal
- Səhifə Blazor Server texnologiyası ilə hazırlanıb və `ITestAutomationService` servisindən istifadə edir
- Loglar yaddaşda saxlanılır və server yenidən başladıqda təmizlənir
- Gələcəkdə nəticələr `ApplicationDbContext` vasitəsilə verilənlər bazasında saxlanıla bilər

