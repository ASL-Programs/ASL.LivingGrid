# LocalizationBook

Bu kitab WebAdminPanel modulunda tərcümə idarəçiliyi və dil paketlərinin yaradılması qaydalarını izah edir.

## Əsas Funksiyalar
- Dil resurslarının toplu redaktəsi
- Versiya tarixi və təsdiqləmə mexanizmi
- JSON və XML formatlarında import və export
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

## TemplateOverride və TerminologyOverride

`TemplateOverride` hər tenant və modul üçün şablon mətnlərini dəyişməyə imkan
verir. `TerminologyOverride` servisi isə terminləri eyni üsulla
fərdiləşdirməyə kömək edir. Bu mexanizm portalın müxtəlif hissələrinin
müxtəlif müştərilər və modullar üçün uyğun termin və şablonlarla işləməsinə
imkan yaradır.

REST API nümunələri:

```
GET /api/localization/customization/templates/{culture}/{module}
POST /api/localization/customization/templates/{culture}/{module}
GET /api/localization/customization/terminology/{culture}/{module}/{key}
POST /api/localization/customization/terminology/{culture}/{module}/{key}
```

Məlumatlar `TemplateOverrides` və `TerminologyOverrides` cədvəllərində
saxlanılır və `LocalizationService` tərəfindən oxunur.

## Örtük Ölçümləri və Keyfiyyət Alətləri

`LocalizationCoverage.razor` səhifəsi hər modul üzrə tərcümə faizini göstərir.
`LocalizationService.GetCoverageByCategoryAsync` metodu əsas dil ilə
müqayisədə tərcümə olunmuş açarların nisbətini hesablayır. Keyfiyyətə nəzarət
üçün `GetMissingKeysAsync`, `ValidatePlaceholdersAsync` və
`GetOverflowStringsAsync` metodları istifadə olunur. `MissingTranslation`
hadisəsi tərcümə tapılmadıqda real vaxt xəbərdarlığı yaradır. Bu alətlər
səhvləri aşkarlayaraq yayımdan əvvəl düzəltməyə kömək edir.

Bu sənəd daim yenilənəcək.

## Dil Paketi Marketplace-i
Dil paketlərini paylaşmaq və reytinq vermək üçün `/marketplace/languagepacks` səhifəsi yaradılıb. Bu səhifədə pack-ları siyahı şəklində görmək, reytinq qoymaq və bir kliklə import/export etmək mümkündür. Məlumatlar `LanguagePackMarketplaceService` vasitəsilə `languagepacks_marketplace.json` faylından və ya konfiqurasiyada göstərilən URL-dən yüklənir.

REST API:
```
GET /api/languagepacks            # mövcud pack-ların siyahısı
GET /api/languagepacks/import/{id}# seçilmiş pack-ı JSON şəklində almaq
GET /api/languagepacks/export/{culture}
POST /api/languagepacks/rate/{id}
```

## Yeniləmələr və Rollback
`LocalizationUpdateService` fon xidməti `pending_languagepack_updates.json` faylını oxuyaraq yeni dil paketlərini mərhələli şəkildə tətbiq edir. Hər yeniləmə `AuditService` vasitəsilə jurnal olunur və `NotificationService` istifadəçilərə bildiriş göndərir. Faylda `Applied` sahəsi yenilənməmiş qeydləri işarələməyə imkan verir.
Yeniləmə yoxlamalarının tezliyi `Localization:UpdateIntervalMinutes` parametrində
dəqiqə olaraq təyin edilir və göstərilmədikdə 30 dəqiqə qəbul olunur.

Rollback üçün əvvəlki dil paketlərini `Export` əməliyyatı ilə saxlayıb istənilən vaxt `Import` edə bilərsiniz.

## Tərcümə Provayderləri üçün API
Üçüncü tərəf provayderləri inteqrasiya etmək məqsədilə `TranslationProviderService` əlavə olunub. Endpoint-lər:
```
GET /api/translationproviders
POST /api/translationproviders       # yeni provayder əlavə et
DELETE /api/translationproviders/{id}
POST /api/translationproviders/webhook/{id}
```
`Webhook` endpoint-i provayderdən gələn yeniləmələri qəbul etmək üçün nəzərdə tutulub.
