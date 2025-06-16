# ReportingBook

Bu sənəd WebAdminPanel modulunda hesabat xidmətinin qurulma məqsədini, istfadə qaydalarını və idarəetmə prinsiplərini əhatə edir.

## Niyə yaradılıb?
- Məlumatların analitik təqdimatı və istənilən kəsimdə filtrlənməsi üçün
- Fərdi və şirkət səviyyəsində hesabatların UI üzərindən yaradılması

## Nəyə xidmət edir?
- İstifadəçilərə qabaqcıl filterlərlə custom hesabatlar hazırlamaq
- Hesabat nəticələrini PDF, Excel, CSV, JSON və Parquet formatında ixrac etmək
- Audit hadisələrini vizual zaman xətti ilə izləmək və SIEM/Syslog sistemlərinə ötürmək
- Rol və tenant səviyyəsində hesabatlara baxış icazələrini təyin etmək

## İstifadə qaydası və idarəetmə prinsipləri
1. **Report siyahısı:** `Reports` səhifəsində mövcud hesabatları seçib filtrləri tətbiq etmək mümkündür.
2. **Export:** İstənilən hesabat nəticəsini uyğun formatda `ExportReportAsync` metodu vasitəsilə əldə etmək olar.
3. **Audit timeline:** Hər hesabat icrasının audit logu `GetAuditTimelineAsync` metodu ilə çıxarılır.
4. **İcazələr:** `AllowedRoles` və tenant parametrinə əsasən yalnız səlahiyyətli istifadəçilər hesabatlara baxa bilər.

## Texniki və biznes üstünlükləri
- **Modulluq:** Yeni hesabatların əlavə edilməsi və filtrlərin genişləndirilməsi asandır.
- **Mərkəzləşmiş audit:** Bütün hərəkətlər izlənir və lazım olduqda SIEM sistemlərinə ötürülür.
- **Çoxformatlı ixrac:** Müxtəlif biznes tələblərinə uyğun məlumat paylaşımı imkanı.

## Gələcək inkişaf yolları və risklər
- Zamanlanmış hesabatların avtomatik göndərilməsi
- Daha dərin təhlükəsizlik yoxlamaları və performans optimizasiyası
- Filtr və sorğu dizaynerinin genişləndirilməsi

## Yeni imkanlar
1. **Hesabat şablonunun saxlanması:** `ReportDesigner` səhifəsində sorğunu `SaveReportAsync` metodu ilə yadda saxlayın.
2. **Zamanlama:** Eyni səhifədə hesabatı gələcək tarixə planlaşdırmaq üçün `ScheduleReportAsync` metodundan istifadə edin.
3. **Arxa plan xidməti:** `ReportSchedulerService` planlaşdırılmış hesabatları icra edir və nəticəni `NotificationService` vasitəsilə göndərir.

