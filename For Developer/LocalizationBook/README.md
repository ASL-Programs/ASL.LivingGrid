# LocalizationBook

Bu kitab WebAdminPanel modulunda tərcümə idarəçiliyi və dil paketlərinin yaradılması qaydalarını izah edir.

## Əsas Funksiyalar
- Dil resurslarının toplu redaktəsi
- Versiya tarixi və təsdiqləmə mexanizmi
- JSON formatında import və export
- REST API vasitəsilə dil paketlərinin idarəsi
- AI tərcümə təklifləri üçün `TranslationWorkflowService.SuggestAsync` metodu
- Tərcümə sorğularının vəziyyətləri: **Machine**, **Human**, **PendingReview**, **MachineApproved**, **HumanApproved**, **Approved**, **Rejected**

## Statusların mənası
- **Machine** – maşın tərcüməsi, ilkin dəyər kimi əlavə edilir və yoxlamaya göndərilir.
- **Human** – insan tərəfindən daxil edilən tərcümə, moderasiya tələb edir.
- **PendingReview** – tərcümə redaktorun təsdiqini gözləyir.
- **MachineApproved** – maşın tərcüməsi redaktor tərəfindən təsdiqlənib.
- **HumanApproved** – insan tərcüməsi redaktor tərəfindən təsdiqlənib.
- **Approved** – mənbə qeyd olunmayan təsdiqlənmiş tərcümə.
- **Rejected** – moderator tərəfindən rədd edilmiş və tətbiq olunmayan tərcümə.
- `TranslationRequest` modelinə `ReviewerComments`, `Escalate`, `RejectedBy` və `RejectedAt` sahələri əlavə olunub


## İstifadə Qaydası
1. `/languagepacks` səhifəsinə keçid edərək mövcud dillərin siyahısını görün.
2. Dil seçib "Export" düyməsi ilə mövcud tərcümələri JSON fayl kimi yükləyin.
3. Eyni formatda faylı seçib yükləyərək `Import` əməliyyatı aparın.
4. API vasitəsilə `/api/localization` endpoint-lərindən də istifadə etmək mümkündür.
5. Tərcümə təklifi üçün `/api/translationrequests/suggest` endpoint-i POST sorğusu göndərin.
   Sorğu bədənində `text`, `sourceCulture` və `targetCulture` sahələrini göndərin.
6. Moderator təsdiqi və ya rədd üçün `/api/translationrequests/review/{id}` endpoint-inə POST sorğusu göndərin.
   Bədənin nümunəsi:
   `{ "accept": true, "comments": "ok", "escalate": false }`
7. Birbaşa təsdiqləmə üçün `/api/translationrequests/approve/{id}` endpointindən istifadə edin.
8. Rədd etmək və səbəb bildirmək üçün `/api/translationrequests/reject/{id}` endpointinə POST sorğusu göndərin.
9. `appsettings.json` faylında `Translation` bölməsində API açarını (`ApiKey`),
   provayderi (`Provider`) və digər parametrləri (`Endpoint`, `Model`, `Region`)
   təyin edin. Artıq OpenAI, DeepL, Google Translate, Azure Translator və
   `Custom` provayderləri mövcuddur. Seçilən provayderə uyğun olaraq endpoint,
   model və region dəyərlərini doldurmağı unutmayın. `Custom` rejimində endpoint
   sizin daxili servisinizə yönəldilir və lazım olduqda `ApiKey` başlıqda `Bearer`
   kimi ötürülür.

10. Moderatorlar üçün `/pendingreviews` Blazor səhifəsi mövcuddur. Bu səhifə
    gözləyən sorğuların siyahısını göstərir və hər sorğu üçün **Approve** və
    **Reject** düymələri vasitəsilə yuxarıdakı API-lərə sorğu göndərir.
11. Hər bir sorğu üçün **Preview** düyməsi mövcuddur. Bu düymə təklif edilən
    tərcüməni `/translationpreview` səhifəsində seçilmiş dil və açarla birlikdə
    real vaxtda göstərir. İstifadəçi tərcüməni yayımlamadan əvvəl necə
    göründüyünü sınaqdan keçirə bilir.

## CultureCustomization servisi

Yeni `CultureCustomizationService` hər dil üçün font ailəsi, ölçü miqyası və
`TextDirection` (LTR və ya RTL) kimi parametrləri saxlamağa imkan verir.

REST API nümunələri:

```
GET /api/localization/customization/{culture}
POST /api/localization/customization/{culture}
```

Bu məlumatlar `CultureCustomizations` cədvəlində saxlanılır və öncəliklə
`LocalizationService` tərəfindən oxunur.

Bu sənəd daim yenilənəcək.
