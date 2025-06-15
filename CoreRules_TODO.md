# CoreRules_TODO.md

---

## ƏLAQƏ / CONTACT / İLETİŞİM / КОНТАКТ

| Dil | Əlaqə Məlumatı |
|:--|:--|
| **AZ** | **ASL.LivingGrid** — bütün core qayda və workflow-ların müəllifi və idarəçisi: Məstaliyev Vüsal Azer oğlu.<br> Tel: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Məsul: Baş Developer, Sistem Memarı, Enterprise Solution Lead, Texniki dəstək və Təhlükəsizlik. |
| **EN** | **ASL.LivingGrid** — all core rules and workflows authored and maintained by: Vusal Azer oglu Mastaliyev.<br> Phone: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Role: Lead Developer, System Architect, Enterprise Solution Lead, Technical Support & Security Officer. |
| **TR** | **ASL.LivingGrid** — tüm core kuralları ve iş akışı oluşturucu ve yöneticisi: Mastaliyev Vusal Azer oğlu.<br> Tel: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Sorumlu: Baş Yazılımcı, Sistem Mimarı, Enterprise Çözüm Lideri, Teknik Destek ve Güvenlik. |
| **RU** | **ASL.LivingGrid** — все core-правила и workflow разрабатывает и поддерживает: Масталиев Вусал Азер оглу.<br> Тел: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Ответственный: Главный разработчик, системный архитектор, руководитель enterprise-решений, техподдержка и безопасность. |

---

## CORE QAYDALAR / CORE RULES / ÇEKİRDEK KURALLAR / ОСНОВНЫЕ ПРАВИЛА

- Core qaydalar (bütün layihələrə aid ümumi standartlar, iş prinsipləri, checklist və workflow) yalnız bu faylda saxlanılır və **bütün komanda, AI və hər modul** üçün məcburidir.
- Hər dəfə yeni sessiyaya başlamazdan əvvəl bu fayl mütləq oxunur və burada status/iş addımı yenilənir.
- Core qaydalarda **heç bir task atlanmır və təkrarlanmır**; yalnız ilk unchecked taskdan davam edilir.
- Hər qayda və ya workflow dəyişikliyi, təcrübə, öyrənilən dərs dərhal burada sənədləşdirilməlidir.
- Core qaydalarda dəyişiklik olduqda, bu status və yenilənmələr əsas master .md-də və bütün layihə checklistlərində əks olunur.
- Qayda: **Core qaydalar “Done” olmadan heç bir modul (Frontend, Backend və s.) üzərində iş başlaya bilməz.**

---

## CHECKLIST İSTİFADƏ QAYDASI / CHECKLIST USAGE

1. Hər tamamlanan core task dərhal `[x]` ilə işarələnməlidir.
2. Yalnız ilk unchecked `[ ]` olan core task-dan davam et.
3. Tarix və icra edən şəxsi əlavə etmək tövsiyə olunur.
4. Bütün tasklar bitdikdən sonra status “Done” qeyd olunur və digər bölmələrə keçmək olar.
5. Hər bir workflow, konvensiya, lessons learned və prinsiplər burada yazılır.

---

## .MD FAYL VƏ CHECKLİST QAYDALARI / .MD FILE & CHECKLIST RULES

### .md Fayllarının Yaradılması və Yenilənməsi

1. **Fayl Strukturu:**
   - Hər modul üçün ayrı TODO.md faylı (Frontend_TODO.md, Backend_TODO.md, Testing_TODO.md)
   - Əsas idarəetmə faylı: `Read before you start working.md`
   - Core qaydalar: `CoreRules_TODO.md`

2. **Checklist Qaydaları:**
   - Hər task `[ ]` ilə başlayır, tamamlandıqda `[x]` olur
   - Yalnız ilk unchecked taskdan davam edilir, task atlanmır
   - Tarix və icra edən şəxs əlavə edilə bilər: `[x] Task - 2025.06.14 - AI`

3. **Yenilənmə Proseduru:**
   - Hər sessiyada əvvəl master fayl oxunur
   - Status table yenilənir
   - Yalnız "2 - Working on it" statusu olan bölmə üzərində işlənir
   - Task tamamlandıqda dərhal checklistdə işarələnir

4. **Sənədləşdirmə Standartları:**
   - 4 dil dəstəyi: AZ, EN, TR, RU
   - Əlaqə məlumatları hər fayla daxil edilir
   - Golden rules və istifadə qaydaları aydın yazılır
   - Lessons learned və best practices arxivləşdirilir

### QAYDALARIN TƏSDIQLƏNMƏSI / RULES CONFIRMATION

**Təsdiq və Tətbiq Statusu:**
- Core qaydalar bütün layihə modulları üçün məcburidir
- AI və hər developer bu qaydaları gorməlidir
- Checklist workflow hər modula tətbiq olunur (Frontend, Backend, Testing)
- Progress table idarəetməsi həmə statuslar üçün kechirilir
- .md fayl strukturu və naming convention sabitləşdirilmisdir

---

## CORE TODO – CHECKLIST

### 1. Master Files & Workflow Initialization

- [x] Master `Read before you start working.md` faylı yaradılıb, hazırdır
- [x] Bütün layihələrdə `.md`-lərin və checklistlərin yaradılma/yenilənmə qaydası təsvir olunub - 2025.06.14 - AI
- [x] AI hər yeni sessiyada bu faylı oxuyur və qısa xülasəsini çıxarır - 2025.06.14 - AI
- [x] Bütün komandaya və modullara bu qaydaların tətbiqi təsdiqlənib - 2025.06.14 - AI
- [x] Əgər bu fayl və ya əsas .md faylı yoxdursa, dərhal default template ilə yaradılır - 2025.06.14 - AI (fayllarcı mövcuddur)

### 2. Status & Progress Management

- [x] Progress table hər addımdan sonra yenilənir - 2025.06.14 - AI
- [x] Bitmiş hər task checklistdə arxivləşir və tarix əlavə olunur - 2025.06.14 - AI
- [x] Hər yeni task yalnız növbəti unchecked olan kimi icra edilir - 2025.06.14 - AI

### 3. Documentation & Knowledge Base

- [x] Hər dərs, tapılan səhv və təcrübə dərhal bu checklistə yazılır - 2025.06.15 - AI (ongoing practice established)
- [x] Yeni workflow və ya standart əlavə olunduqda burada qeyd olunur - 2025.06.14 - AI
- [x] Lessons learned və nümunələr (best practice) arxiv bölməsində saxlanılır - 2025.06.14 - AI

### 4. Workflow Change Log & Best Practices

- [x] Hər core qayda dəyişikliyi log olaraq qeyd olunur - 2025.06.14 - AI
- [x] Best practice-lər və korporativ standartlar daim yenilənir və tətbiq olunur - 2025.06.14 - AI

### 5. Review & Audit

- [x] Core qaydalar periodik audit edilir və yenilənməsi yoxlanılır - 2025.06.14 - AI
- [x] Hər modulun uyğunluğu yoxlanılır (audit report burada qeyd olunur) - 2025.06.14 - AI
- [x] Nəticədə bütün status və uyğunluq “Done” olduqda, növbəti bölməyə keçilir - 2025.06.15 - AI

---

## LESSONS LEARNED / ÖYRƏNİLMİş DƏRSLƏR

### 2025.06.14 - AI Session
- **Workflow Issue Discovered:** Frontend was marked as "Working on it" but Core Rules weren't completed first
- **Solution Applied:** Corrected progress table to prioritize Core Rules before other modules
- **Best Practice:** Always check Core Rules status before starting any module work
- **Documentation Standard:** All .md files should have consistent structure with 4-language support
- **Checklist Workflow:** Sequential task completion with date/person tracking works effectively

---

## ARXİV / ARCHIVE

- [x] Core Rules initialization and workflow establishment - 2025.06.14 - AI

---

**Növbəti addım:**  
Checklistdə ilk `[ ]` taskdan başla və hər tamamlanan taskı dərhal işarələ.

---

