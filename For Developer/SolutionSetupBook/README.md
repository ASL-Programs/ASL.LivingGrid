# SolutionSetupBook

Bu kitab ASL.LivingGrid həllinin yaradılma səbəblərini, layihəyə necə xidmət etdiyini və idarəetmə prinsiplərini izah edir. Sənəd tam Azərbaycan dilində saxlanılır, texniki terminlər isə ingilis dilində verilir.

## Niyə yaradılıb?
ASL.LivingGrid platforması çoxsaylı modul və tətbiqlərdən ibarətdir. "SolutionSetupBook" bu modulların vahid strukturda toplandığı `ASL.LivingGrid.sln` və müstəqil həllər üçün əsas quruluş prinsiplərini izah etmək üçün yaradılıb. Məqsəd hər bir developera başlanğıc mərhələdən aydın yol xəritəsi vermək və layihə boyu ardıcıllığı qorumaqdır.

## Nəyə xidmət edir?
Bu sənəd developerlərin yeni modul və ya tətbiq əlavə edərkən aşağıdakı suallara cavab tapmasına kömək edir:
- Hansı .sln faylı hansı modula aiddir?
- Paylaşılan `Shared` kitabxanasından necə istifadə olunmalıdır?
- Yeni modulun `For Developer` qovluğunda öz Book sənədi necə yaradılmalıdır?

## İstifadə qaydası və idarəetmə prinsipləri
1. **Müstəqil Solution**: Hər modul (WebAdminPanel, ManagerPanel və s.) ayrıca .sln kimi yaradılır və saxlanılır. Kod paylaşımı yalnız `Shared` paketləri və ya NuGet vasitəsilə həyata keçirilir.
2. **Mərkəzi həll**: Kökdəki `ASL.LivingGrid.sln` bütün modulların referensini saxlayır və ümumi build üçün istifadə olunur.
3. **Build skriptləri**: `Scripts` qovluğunda yerləşən `.ps1` və `.sh` faylları vasitəsilə həm tək, həm də bütün həll üzrə build/restore əməliyyatları aparılır.
4. **Config idarəetməsi**: Bütün parametrlər UI-dan redaktə edilə bilən konfiqurasiya xidmətləri üzərindən saxlanılır; `appsettings.json` yalnız ilkin dəyərlər üçündür.
5. **Doc yeniləmə**: Hər modulda dəyişiklik etdikdə müvafiq "Book" sənədi yenilənməlidir.

## Texniki və biznes üstünlükləri
- **Şəffaf struktur**: Müstəqil solution-lar sayəsində hər modul ayrıca inkişaf etdirilir, test olunur və yerləşdirilir.
- **Asan miqyaslama**: Yeni modul əlavə etmək üçün sadəcə yeni .sln yaradıb kök həllə əlavə etmək kifayətdir.
- **UI yönümlü konfiqurasiya** istifadəçi və administratorlar üçün daha çevik və təhlükəsiz idarəetmə imkanı yaradır.
- **Daha sürətli devops**: Modul səviyyəsində build və deploy prosesləri sadələşir, CI/CD boru xətləri rahat qurulur.

## Gələcək inkişaf yolları və risklər
- **Bulud miqrasiyası**: Hər modulun konteyner əsaslı yerləşdirilməsi və orchestrator (Kubernetes, Docker Swarm) dəstəyi üzərində işlər planlaşdırılır.
- **Monorepo riskləri**: Bütün həllin eyni repoda saxlanması zamanla ölçü və idarəetmə çətinlikləri yarada bilər; modul bağımsızlığı qorunmalıdır.
- **Versiya uyğunluğu**: `Shared` kitabxanası ilə modullar arasında API dəyişiklikləri üçün versiyalaşdırma strategiyası müəyyənləşdirilməlidir.
- **Yeni texnologiyalar**: .NET və MAUI versiya yenilikləri çıxarsa, bütün solution-lar yenidən tərtib olunmalı və sınaqdan keçirilməlidir.

Bu sənəd mütəmadi yenilənəcək və hər bir developer yeni modul əlavə etdikdə buradakı prinsiplərə əməl etdiyini yoxlamalıdır.
