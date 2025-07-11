# UXUIBook

Bu sənəd WebAdminPanel modulunun istifadəçi təcrübəsi və dizayn prinsiplərini izah edir.
### Qısa xülasə
`Theme Marketplace` səhifəsi aktivdir və aşağıdakı imkanları təqdim edir:
- Mövcud temaları siyahıdan seçib dərhal tətbiq etmək.
- Yeni tema faylı yükləyib `/api/themes/import/{id}` endpointi ilə aktivləşdirmək.
- İstənilən temanı ixrac edib başqa mühitdə istifadə etmək.

## Məqsəd
- İstifadəçilərə bütün cihazlarda rahat görünən adaptiv interfeys təqdim etmək.
- Şirkətlər üçün fərdiləşdirilə bilən tema və brendinq imkanları yaratmaq.
- Əlçatanlıq (accessibility) tələblərinə cavab vermək və canlı UI audit funksiyası təmin etmək.

## Əsas Prinsiplər
1. **Responsiv Dizayn:** Bootstrap 5 əsasında sidebar və əsas məzmun mobil və masaüstü baxışa uyğun olaraq kollaps olur.
2. **Tema Dəstəyi:** `ThemeService` vasitəsilə light/dark rejimi seçilir və istifadəçinin brauzerində yadda saxlanılır.
3. **Brendinq:** Rəng və loqo kimi parametrlər gələcəkdə `IUICustomizationService` üzərindən genişlənə bilər.
4. **Əlçatanlıq:** `UIAudit` səhifəsi sadə skriptlə şəkil alt mətnləri və label uyğunsuzluqlarını yoxlayır.
- `Theme Marketplace` səhifəsində mövcud temalar siyahılanır, yeni tema faylı yüklənir və mövcud tema ixrac edilə bilir.
- Tema Marketplace səhifəsi ilə mövcud temaları siyahıdan seçmək, yeni tema yükləmək və mövcud temaları ixrac etmək mümkündür.
5. **Dinamik Navigasiya:** `NavigationService` menyu elementlərini `menuitems.json` faylından oxuyur və `NavMenu` komponentində dinamik şəkildə göstərir. Faylı dəyişdirməklə menyu yenilənir və istənilən tenant üçün fərdi struktur qurmaq mümkündür.
6. **Mikro İnteraksiyalar:** `toast.js` vasitəsilə istifadəçi ilk dəfə ana səhifəyə daxil olduqda xoş gəlmisiniz bildirişi göstərilir.

## İstifadə Qaydası
- `ThemeToggle` komponenti vasitəsilə istifadəçi istənilən vaxt temanı dəyişə bilər.
- UI audit funksiyası menyuda **"UI Audit"** bölməsi altında yerləşir.
  1. Səhifəni açdıqdan sonra **"Run Audit"** düyməsini sıxın.
  2. Skript bütün `img` və `input` elementlərini yoxlayaraq alt mətn və label əlaqələrini analiz edəcək.
  3. Tapılan uyğunsuzluqlar siyahıda göstəriləcək və heç bir problem yoxdursa "No issues found" mesajı görünəcək.
  - "Tema Bazarı" səhifəsində mövcud şablonları siyahı şəklində görmək və bir düymə ilə tətbiq etmək mümkündür. Seçilən tema `/api/themes/import/{id}` endpointi vasitəsilə yüklənir və `ThemeService` ilə aktiv edilir.

### Axtarış
- İndi həm yuxarı paneldə, həm də sol menyuda **SearchBox** mövcuddur.
- Sorğu yazdıqca təkliflər ani olaraq açılır və nəticələr konfiqurasiya açarları, istifadəçilər, modullar və sənədlər üzrə qruplanır.
- `SearchService` bu kateqoriyaların hər birini indekslədiyi üçün nəticələr bir sorğu ilə yüklənir.
- Sorğunu təmizlədikdə siyahı gizlənir.

### Dashboard Dizayner
- Menyudan **"Dashboard Designer"** səhifəsinə daxil olun.
- Soldakı siyahıdan mövcud vidjetləri sürükləyib kətan bölməsinə buraxın.
- Vidjet əlavə olunduqdan sonra yerləşdirməni dəyişmək və ya silmək üçün sadə drag-and-drop funksiyası aktivdir.
- Hazır düzən `dashboardDesigner.js` skripti vasitəsilə brauzer yaddaşında saxlanılır və növbəti açılışda bərpa olunur.

### NavigationService ilə Rol və Tenant Menyusu
- `NavigationService` artıq `RoleBasedUiService` vasitəsilə istifadəçi rollarına uyğun menyu elementlərini qaytarır.
- Tenant üçün `menuitems.{tenant}.json` faylı tapılarsa, əsas menyunu həmin fayl əvəz edir.
- `NavMenu` komponenti bu servisdən `GetMenuItemsAsync(user, tenantId)` çağırışı ilə siyahını alır.

### WYSIWYG Redaktor
- `VisualEditor` səhifəsi `contenteditable` əsaslı sadə redaktorla yenilənib.
- `wysiwyg.js` skripti daxilində yazılan mətnləri dərhal ifrəmədə göstərir.
- **B**, **I**, **U** düymələri `execCommand` ilə mətni formatlayır.

## Gələcək İnkişaf
- Tam WCAG 2.1 uyğunluğunun avtomatik yoxlanılması üçün genişlənmiş audit modulu.
- Fərdi şirkətlər üçün rəng palitrası və loqo yükləmə imkanları.
- Drag-and-drop əsaslı tərtibat redaktoru və canlı önizləmə.
- Tema Marketplace səhifəsi ilə mövcud temaları siyahıdan seçmək, yeni tema yükləmək və mövcud temaları ixrac etmək mümkündür.
