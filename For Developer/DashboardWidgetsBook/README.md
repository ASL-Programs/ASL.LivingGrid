# DashboardWidgetsBook

Bu sənəd WebAdminPanel modulunda dashboard və vidjet sisteminin yaradılma məqsədini, istifadəsini və idarəetmə prinsiplərini izah edir.

### Qısa xülasə
`Dashboard` funksiyası hər istifadəçi və şirkət üçün fərdiləşdirilən panel yaratmağa imkan verir. Vidjetləri sürüklə-burax üsulu ilə əlavə etmək, silmək və düzənləmək mümkündür. Vidjet bazarı (marketplace) vasitəsilə yeni vidjetlər yüklənir və istifadəyə verilir. Geniş icazə sistemi sayəsində hər vidjetin hansı modul, tenant və istifadəçi üçün əlçatan olduğunu təyin etmək olur.

## Niyə yaradılıb?
- Məlumatları real vaxtda izləmək və qərarverməni sürətləndirmək.
- İstifadəçinin ehtiyacına uyğun modul və göstəriciləri bir yerdə toplamaq.
- Sistemə sonradan əlavə olunan modulların öz vidjetlərini bazar vasitəsilə paylaşması.

## Nəyə xidmət edir?
- Şirkət və istifadəçilər üçün fərdi dashboardlar qurmağa.
- Vidjetlərin icazə əsasında paylanmasına və idarə olunmasına.
- Real-time bildiriş və statistika göstərməsinə.

## İstifadə qaydası və idarəetmə prinsipləri
1. **Vidjet Bazarı:** `WidgetsMarketplace` səhifəsindən vidjetləri seçmək, reytinqə baxmaq və bir kliklə import etmək mümkündür.
2. **İcazə Sistemi:** `WidgetPermissionService` hər vidjet üçün modul, tenant və istifadəçi səviyyəsində icazələri yoxlayır.
3. **Panelin İdarə Olunması:** İstifadəçi `DashboardDesigner` üzərindən vidjetləri sürükləyərək yerləşdirir, ölçülərini dəyişir və saxlayır.
4. **Audit:** Bütün dəyişikliklər `WidgetAuditLog` cədvəlində qeydə alınır.

## Texniki və biznes üstünlükləri
- **Modulluq:** Yeni vidjetləri bazar vasitəsilə əlavə etmək, kod dəyişmədən funksionallığı genişləndirmək imkanı verir.
- **Dəqiq İcazə Nəzarəti:** Hər vidjet üçün granular icazə sayəsində məlumatın konfidensiallığı qorunur.
- **Real-Time:** Planlaşdırılan `RealtimeWidgetHub` vidjetləri istifadəçi baxışında anlıq yeniləməyə imkan verəcək.
- **Biznes Dəyəri:** İstifadəçilər öz roluna uyğun məlumatları dərhal görərək daha çevik qərar verirlər.

## Gələcək inkişaf yolları
- Vidjetlər üçün canlı data axını təmin edən `RealtimeWidgetHub` servisinin tam inteqrasiyası.
- Marketplace üzərində ödənişli vidjetlərin satış və lisenziyalaşdırma mexanizmi.
- İcazələrin analitikası və istifadə statistikasının genişləndirilməsi.

## Risklər
- Vidjetlərin həddindən artıq çox olması performansa təsir göstərə bilər.
- Yanlış icazə tənzimləməsi gizli məlumatın görünməsinə səbəb ola bilər.
- Marketplace-dən yüklənən vidjetlərin təhlükəsizlik testləri aparılmasa, zərərli kod riski var.
