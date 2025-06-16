# WorkflowBook

Bu sənəd WebAdminPanel modulunda vizual workflow designer və form builder sisteminin yaradılma məqsədini, istifadə prinsiplərini və üstünlüklərini izah edir.

### Qısa xülasə
`WorkflowDesignerService` workflow və form elementlərini yaradıb ardıcıl təsdiq (multi-step approval) mərhələlərini idarə etməyə imkan verir. Hər workflow üçün C#, Python və JavaScript dillərində script triggerləri təyin etmək mümkündür. Form sahələri üçün real-time validasiya qaydaları əlavə olunur və istifadəçi məlumatı anında yoxlanılır.

## Niyə yaradılıb?
- Biznes proseslərini kod yazmadan avtomatlaşdırmaq.
- Təsdiq mərhələlərini və form doldurma qaydalarını mərkəzləşdirmək.
- Fərdi skriptlər ilə sistemə inteqrasiya imkanını genişləndirmək.

## Nəyə xidmət edir?
- İstifadəçi tərəfindən sürüklə-burax üsulu ilə form sahələri və workflow addımları yaratmağa.
- Hər addım üçün səlahiyyətli şəxsləri təyin edib çoxmərhələli təsdiq axını qurmağa.
- Məlumat daxil edilərkən dərhal regex əsaslı validasiya aparmağa.
- Hər workflow hadisəsinə uyğun C#, Python və ya JS skriptlərini işə salmağa.

## İstifadə qaydası və idarəetmə prinsipləri
1. **Workflow yaradılması:** `CreateWorkflowAsync` metodu ad və təsvir qəbul edərək yeni workflow qaytarır.
2. **Form sahəsi əlavə edilməsi:** `AddFormFieldAsync` metodu sahə parametrlərini qəbul edir və workflow-a daxil edir.
3. **Təsdiq addımı:** `AddApprovalStepAsync` ilə mərhələləri ardıcıllıqla qeyd etmək olur.
4. **Validasiya qaydaları:** `AddValidationRuleAsync` form sahəsinə regex və xəta mesajı təyin edir.
5. **Skript triggeri:** `TriggerScriptAsync` workflow hadisəsi baş verdikdə seçilmiş dil üzrə skriptin işə düşməsini təmin edir.

## Texniki və biznes üstünlükləri
- **Esneklik:** İstənilən biznes prosesini kod dəyişmədən qurmağa imkan verir.
- **Təhlükəsizlik:** Təsdiq mərhələləri və qaydalar vahid mərkəzdə saxlanılır, audit üçün rahatlıq yaradır.
- **Gələcək genişlənmə:** Skript dili dəstəyi sayəsində inteqrasiya imkanları açıq qalır.

## Gələcək inkişaf yolları və risklər
- Skriptlərin sandbox mühitində icrası və nəticələrin izlənməsi.
- Workflow şablonlarının marketplace-də paylaşılması.
- Yanlış regex və ya skriptlərin performansa təsir etməməsi üçün limitlər tətbiqi.
