# UXUIBook

Bu sənəd WebAdminPanel modulunun istifadəçi təcrübəsi və dizayn prinsiplərini izah edir.

## Məqsəd
- İstifadəçilərə bütün cihazlarda rahat görünən adaptiv interfeys təqdim etmək.
- Şirkətlər üçün fərdiləşdirilə bilən tema və brendinq imkanları yaratmaq.
- Əlçatanlıq (accessibility) tələblərinə cavab vermək və canlı UI audit funksiyası təmin etmək.

## Əsas Prinsiplər
1. **Responsiv Dizayn:** Bootstrap 5 əsasında sidebar və əsas məzmun mobil və masaüstü baxışa uyğun olaraq kollaps olur.
2. **Tema Dəstəyi:** `ThemeService` vasitəsilə light/dark rejimi seçilir və istifadəçinin brauzerində yadda saxlanılır.
3. **Brendinq:** Rəng və loqo kimi parametrlər gələcəkdə `IUICustomizationService` üzərindən genişlənə bilər.
4. **Əlçatanlıq:** `UIAudit` səhifəsi sadə skriptlə şəkil alt mətnləri və label uyğunsuzluqlarını yoxlayır.
5. **Dinamik Navigasiya:** `NavigationService` menyu elementlərini `menuitems.json` faylından oxuyur və `NavMenu` komponentində dinamik şəkildə göstərir. Bu, tenant və ya istifadəçi səviyyəsində fərqli menyu qurmağa imkan verir.
6. **Mikro İnteraksiyalar:** `toast.js` vasitəsilə istifadəçi ilk dəfə ana səhifəyə daxil olduqda xoş gəlmisiniz bildirişi göstərilir.
7. **Dashboard Dizaynı:** `DashboardDesigner` səhifəsi drag-and-drop prinsipi ilə işləyir və `dashboardDesigner.js` vasitəsilə real vaxt önizləmə imkanı verir.
8. **Tərcümə İş Axını:** `TranslationWorkflowService` istifadəçilərin tərcümə təkliflərini göndərməsi və moderatorun təsdiq etməsi üçün REST API təqdim edir. Bütün əməliyyatlar `AuditService` ilə qeyd olunur.


## İstifadə Qaydası
- `ThemeToggle` komponenti vasitəsilə istifadəçi istənilən vaxt temanı dəyişə bilər.
- UI audit funksiyası menyuda "UI Audit" bölməsi altında yerləşir və nəticələri siyahı şəklində göstərir.

## Gələcək İnkişaf
- Tam WCAG 2.1 uyğunluğunun avtomatik yoxlanılması üçün genişlənmiş audit modulu.
- Fərdi şirkətlər üçün rəng palitrası və loqo yükləmə imkanları.
- Drag-and-drop əsaslı tərtibat redaktoru və canlı önizləmə.
