# LocalizationBook

Bu kitab WebAdminPanel modulunda tərcümə idarəçiliyi və dil paketlərinin yaradılması qaydalarını izah edir.

## Əsas Funksiyalar
- Dil resurslarının toplu redaktəsi
- Versiya tarixi və təsdiqləmə mexanizmi
- JSON formatında import və export
- REST API vasitəsilə dil paketlərinin idarəsi
- AI tərcümə təklifləri üçün `TranslationWorkflowService.SuggestAsync` metodu
- Tərcümə sorğularının vəziyyətləri: **Machine**, **Human**, **PendingReview**, **Approved**
- `TranslationRequest` modelinə `ReviewerComments` və `Escalate` sahələri əlavə olunub

## İstifadə Qaydası
1. `/languagepacks` səhifəsinə keçid edərək mövcud dillərin siyahısını görün.
2. Dil seçib "Export" düyməsi ilə mövcud tərcümələri JSON fayl kimi yükləyin.
3. Eyni formatda faylı seçib yükləyərək `Import` əməliyyatı aparın.
4. API vasitəsilə `/api/localization` endpoint-lərindən də istifadə etmək mümkündür.
5. Tərcümə təklifi üçün `/api/translationrequests/suggest` endpoint-i POST sorğusu göndərin.
   Sorğu bədənində `text`, `sourceCulture` və `targetCulture` sahələrini göndərin.
6. Moderator təsdiqi üçün `/api/translationrequests/review/{id}` endpoint-inə POST sorğusu göndərin.
   Bədənin nümunəsi:
   `{ "accept": true, "comments": "ok", "escalate": false }`
7. `appsettings.json` faylında `Translation` bölməsində API açarını (`ApiKey`)
   və provayderi (`Provider`, `Endpoint`, `Model`) təyin edin. Hazırda OpenAI,
   DeepL və Google Translate dəstəklənir. Provayderə uyğun endpoint və model
   dəyərlərini doldurmağı unutmayın.

Bu sənəd daim yenilənəcək.
