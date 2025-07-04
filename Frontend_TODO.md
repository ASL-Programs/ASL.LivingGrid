# Frontend_TODO.md

---

## ƏLAQƏ / CONTACT / İLETİŞİМ / КОНТАКТ

| Dil | Əlaqə Məlumatı |
|:--|:--|
| **AZ** | **ASL.LivingGrid** — sistem və bütün modullar “ASL, Məstaliyev Vüsal Azer oğlu” tərəfindən hazırlanır və idarə olunur.<br> Tel: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Məsul: Baş Developer, Sistem Memarı, Enterprise Solution Lead, Texniki dəstək və Təhlükəsizlik. |
| **EN** | **ASL.LivingGrid** — all modules are designed and maintained by "ASL, Vusal Azer oglu Mastaliyev".<br> Phone: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Role: Lead Developer, System Architect, Enterprise Solution Lead, Technical Support & Security Officer. |
| **TR** | **ASL.LivingGrid** — tüm modüller “ASL, Mastaliyev Vüsal Azer oğlu” tarafından geliştirilmekte ve yönetilmektedir.<br> Tel: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Sorumlu: Baş Yazılımcı, Sistem Mimarı, Enterprise Çözüm Lideri, Teknik Destek ve Güvenlik. |
| **RU** | **ASL.LivingGrid** — все модули разработаны и поддерживаются “ASL, Масталиев Вусал Азер оглу”.<br> Тел: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Ответственный: Главный разработчик, системный архитектор, руководитель enterprise-решений, техподдержка и безопасность. |

---

## PROQRAM HAQQINDA / ABOUT PROGRAM / PROGRAM HAKKINDA / О ПРОГРАММЕ

| Dil | Məlumat / Info |
|:--|:--|
| **AZ** | **ASL.LivingGrid** — innovativ, təhlükəsiz və tam modullu enterprise idarəetmə və analitika platformasıdır. Hər modul UI-dan idarə olunur, dörd dildə işləyir, genişlənə bilər və istifadəçi dostudur. Məqsəd: Rəqəmsal idarəetməni yeni səviyyəyə çatdırmaq! |
| **EN** | **ASL.LivingGrid** — an innovative, secure and fully modular enterprise management and analytics platform. Every module is UI-driven, multilingual, extensible, and user-centric. Mission: Take digital management to the next level! |
| **TR** | **ASL.LivingGrid** — yenilikçi, güvenli ve tam modüler bir kurumsal yönetim ve analitik platformdur. Tüm modüller UI odaklı, dört dil destekli, genişletilebilir ve kullanıcı dostudur. Amaç: Dijital yönetimi yeni bir seviyeye taşımak! |
| **RU** | **ASL.LivingGrid** — инновационная, безопасная и полностью модульная корпоративная платформа для управления и аналитики. Все модули управляются через UI, поддерживают четыре языка, расширяемы и ориентированы на пользователя. Миссия: вывести цифровое управление на новый уровень! |

---

## DÖRD DİL DƏSTƏYİ / 4 LANGUAGE SUPPORT / 4 DİL DESTEĞİ / ПОДДЕРЖКА 4 ЯЗЫКОВ

- Bütün proqramlar (WebAdmin, Manager, SmartCustomer, ReportingDesktop, ReportingMobile) dörd dili tam dəstəkləməlidir: Azərbaycan (AZ), English (EN), Türkçe (TR), Русский (RU)
- İstifadəçi interfeysi, bildirişlər, kömək və ayarlar menyusu seçilmiş dildə işləməlidir.
- Dil seçimi istənilən vaxt UI-dan dəyişdirilə bilməlidir.
- Hər yeni modul əlavə olunduqda, dil resursları və tərcümələr dərhal daxil edilməlidir.
- Əlaqə və proqram haqqında bölməsi hər dildə göstərilməlidir.

---

## QIZIL QAYDALAR / GOLDEN RULES / ALTIN KURALLAR / ЗОЛОТЫЕ ПРАВИЛА

- Hər bir modul (WebAdmin, Manager, SmartCustomer, ReportingDesktop, ReportingMobile) tam müstəqil Visual Studio solution (.sln) kimi yaradılır və saxlanılır.
- Heç bir kod, config və ya dependency başqa bir solution ilə paylaşılmır. Əlaqə yalnız REST API və ya NuGet package vasitəsilə mümkündür.
- Bütün parametrlər, dillər, endpoint-lər, istifadəçi və şirkət ayarları YALNIZ UI-dan idarə olunur, kodda dəyişiklik edilməz.
- Hər bir task, tamamlananda dərhal `[x]` işarələnməlidir (istəyə görə tarix və icra edən şəxsin adı qeyd oluna bilər).
- AI və developer yalnız ilk `[ ]` task-dan davam etməli, heç bir addım atlanmamalı, heç bir task təkrarlanmamalıdır.
- “Current Step in Progress” bölməsi həmişə növbəti task-ı göstərməlidir.
- Bütün tasklar və modullar enterprise səviyyədə təhlükəsizlik, plugin/extensibility, test və audit tələblərinə cavab verir.
- Checklistlər və statuslar .md fayllarında saxlanılır – komanda və AI həmişə harda qaldığını və növbəti addımı aydın görür.

---

## CHECKLIST İSTİFADƏ QAYDASI / CHECKLIST USAGE

1. Hər tamamlanan task checklist-də dərhal `[x]` ilə işarələnməlidir.
2. Yalnız ilk unchecked `[ ]` olan task-dan davam et, task atlama və təkrarlama olmaz.
3. Tarix və icra edən şəxin adını əlavə etmək tövsiyə olunur.
4. Bütün tapşırıqlar tamamlandıqdan sonra status table-da “Done” işarələnməlidir və növbəti bölməyə keçilməlidir.
5. “Current Step in Progress” həmişə növbəti task-ı göstərməlidir.
6. Bu yanaşma AI və insan üçün təkrarsız, itkisiz, ardıcıl və 100% izlənə bilən iş prosesini təmin edir.

---

## MODUL ÇEKLİSTLƏRİ / MODULE CHECKLISTS

---
# 1. Web Admin Panel (Blazor, Kestrel, EXE, Hosting) — 2025 Ultra-Enterprise Checklist

> **Her başlanğıcda AdminPanel üçün edəcəklərinin və etdiklərinin nələr olduğunu aşağıdakı siyahıdan tam oxu, sonra qaldığın yerdən davam et. (Yazdıqlarını və edəcəklərini unutmayasan!)**

**Qeyd:**  
> Bu panel istənilən ölçüdə layihə üçün (e-ticarət, taxi, məktəb, marketplace, legal, kargo, telekom, franchise, AI SaaS, IoT, RPA, Web3, enerji, tibb, və s.)  
> tam universal, ultra-modular, açıq/kilitli, lisenziyaya görə fərqli görünüş və funksionallıq verə bilən,  
> gələcəkdə istənilən yeni texnologiya və biznes modelini problemsiz əlavə etməyə hazır ultra-enterprise idarəetmə mərkəzidir!

---

## 1.0. Deployment & Architecture Options
- [x] **Self-hosted .exe with Kestrel** (2025-06-15 - AI: Basic .NET 9.0 Blazor project with Kestrel hosting capability created)
 - [x] **Classic Web Hosting** (IIS, Apache, Nginx, VM, Docker, Kubernetes) - 2025-06-15 - AI: Dockerfile və web.config əlavə edildi
 - [x] **Switchable Mode:** .exe <-> Hosting, instant migration, backup/restore - 2025-06-15 - AI: Hosting mode switch implemented in Program.cs
 - [x] **Multi-instance, multi-mode sync** (cloud, on-prem, edge) - 2025-06-15 - AI: SyncService ilə `sync_nodes.json` üzrə periodik sinxronizasiya
 - [x] **Distributed, Hybrid Cloud, Serverless Functions** - 2025-06-17 - AI: DisasterRecoveryService və DeploymentBook yeniləndi
    - Multi-cloud (AWS, Azure, GCP), serverless FaaS, auto-provisioned
    - **Disaster Recovery:** Real-time failover, auto backup, cross-region replication
    - **Edge Deployment:** IoT/branch office, limited-connectivity support

**For Developer qovluğu və Book:**  
Hər dəfə deployment və arxitektura ilə bağlı dəyişiklik və ya yeni bir seçim əlavə olunduqda, `For Developer` qovluğunda ən müasir documentation texnologiyası (məsələn, Docusaurus və ya Docsify) ilə `DeploymentBook` yaradılmalı və ya yenilənməlidir. Burada **bütün addımlar, seçim səbəbləri, risk və üstünlüklər, istifadə və idarəetmə qaydaları, gələcək inkişaf variantları** yalnız Azərbaycan dilində (terminlər istisna) izah olunmalıdır.
- [x] DeploymentBook-a hosting rejimlərinin xülasəsi əlavə olundu - 2025-06-17 - AI
---

## 1.1. Design & User Experience (UX/UI Principles)
 - [x] Fully responsive layout (desktop/tablet/mobile/ultra-wide)
 - [x] Modern, minimal, per-tenant branding & theming
 - [x] Adaptive light/dark, high-contrast, WCAG/ADA
 - [x] Dynamic navigation (sidebar/topbar/hamburger/quick search) - 2025-06-15 - AI: NavigationService və menuitems.json ilə dinamik menyu
 - [x] Micro-interactions, onboarding, live hints, self-personalize - 2025-06-15 - AI: Toast notification on first visit, onboarding wizard
 - [x] Modular, white-label, instant preview, drag-and-drop dashboards - 2025-06-15 - AI: DashboardDesigner komponenti ilə sürüklə-burax düzən
 - [x] **Marketplace for themes/layouts, instant import/export**
 - [x] **Live UI “audit”, accessibility scanner, design error finder**
 - [x] Smart global search (every setting, user, module, log, doc, etc.) - 2025-06-15 - AI
 - [x] Role-based UI shaping & permission simulation - 2025-06-16 - AI
 - [x] **Realtime visual editing, live WYSIWYG preview** - 2025-06-16 - AI
 - [x] **Per-module UI customization and override for each tenant/company** - 2025-06-16 - AI
 - [x] **In-app feedback loop, user satisfaction analytics** - 2025-06-16 - AI
 - [x] **Session persistence:** Auto-save forms, restore previous state after logout/crash - 2025-06-16 - AI

**For Developer qovluğu və Book:**  
`For Developer` qovluğunda `UXUIBook` yaradılmalı və ya mövcud olan sənəd hər dəyişiklikdə yenilənməlidir. Burada dizayn qərarlarının səbəbləri, əsas UI/UX prinsipləri, istifadəçi təcrübəsinin niyə belə seçildiyi, hər bir özəlliyin istifadə və idarəetmə qaydası, audit və gələcək inkişaf planları **ətraflı və yalnız Azərbaycan dilində** yazılmalıdır.
- [x] UXUIBook-a Tema Marketplace istifadəsinin xülasəsi əlavə olundu - 2025-06-17 - AI
---

### 1.1.1. Multi-Language & Translation Lifecycle Management (Enterprise Detailed)
- [x] **Dynamic Language Packs Management** - 2025-06-15 - AI: Import/export page and API implemented
    - [x] Add/edit/clone/preview/version/export/import/audit for each language
    - [x] Bulk edit, AI-assisted batch translate, placeholder protection
    - [x] Language pack versioning, rollback, changelog, audit history
    - [x] Instant test/preview (publish/unpublish without downtime)
    - [x] Export/import: JSON, XML, RESX, YAML, Excel, industry-standard formats
    - [x] Multi-level (UI label, help text, error, system messages, dynamic module strings)
- [x] **Per-Tenant/Per-User Language & Locale Support** - 2025-06-15 - AI: LocalizationService supports company/tenant overrides
    - [x] Tenant-level enable/disable, user override, default/fallback, per-module or per-feature localization
    - [x] Locale-aware formats (date, time, currency, pluralization)
    - [x] Per-branch or per-region language policy enforcement
    - [x] **Collaborative & AI-Assisted Translation** - 2025-06-15 - AI: TranslationWorkflowService əlavə edildi
    - [x] Crowdsource editor: role-based translation request & approval workflow
    - [x] AI translation suggestions (OpenAI, Google, DeepL, custom LLM)
    - [x] Translation status: “machine”, “human”, “pending review”, "machine approved", "human approved", “approved” - 2025-06-18 - AI
    - [x] Activity log for all translation events
    - [x] Proofreading workflow (review, approve, escalate issues) - 2025-06-18 - AI: Proofreading page & approval API

 - [x] **Coverage & Quality Assurance**
    - [x] Coverage dashboard (% complete, missing keys, per-module coverage)
    - [x] Live alert for untranslated/missing/incomplete/invalid placeholders
    - [x] Context preview: see translation in UI before publish - 2025-06-17 - AI
    - [x] Consistency checker: identical terms across modules, placeholder validation - 2025-06-17 - AI
    - [x] Length/overflow check (UI fit) - 2025-06-17 - AI
- [x] **Localization Customization**
    - [x] RTL/LTR, per-language font/size, icon/text direction, culture customization - 2025-06-17 - AI: Added CultureCustomization service & API
    - [x] Custom localized templates per company/tenant/sector/module - 2025-06-19 - AI
    - [x] Tenant-specific terminology overlay - 2025-06-19 - AI
- [x] **Language Pack Marketplace & Sharing** - 2025-06-20 - AI
    - [x] Public/private packs, rate/usage stats, history/changelog, share/import/export with one click
    - [x] Community-contributed packs and tenant-only private packs
- [x] **Automated Update, Notification & Rollback** - 2025-06-20 - AI
    - [x] Notify on update, approve before apply, staged rollout, instant rollback
    - [x] Full audit trail (who/when/what changed)
- [x] **Integration & API** - 2025-06-20 - AI
    - [x] REST API for CRUD, version fetch, 3rd-party translation provider integration (Google, Azure, DeepL)
    - [x] Webhook for translation update events
- [x] **Localization Coverage Dashboard**
    - [x] Per-module, per-tenant, per-user, real-time coverage stats, history, and quality metrics

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.2. Solution & Setup
- [x] Full scaffold (Blazor Server, WASM, MVC) (2025-06-15 - AI: Blazor Server project created with Entity Framework, services, models)
- [x] Only UI-editable config (no hardcode) (2025-06-15 - AI: ConfigurationService created for dynamic UI-based configuration management)
- [x] Tray icon, dual-launch, minimize-to-tray (2025-06-15 - AI: TrayIconService created with Windows Forms integration, dual-launch via Launcher.bat)
- [x] Migration/import/export, onboarding wizard (2025-06-15 - AI: MigrationService and OnboardingService created with comprehensive data management)
- [x] **Zero-touch installer (click-to-install, self-healing, auto-repair)** - 2025.06.15 - AI: InstallerService with comprehensive installation, repair, and health check functionality
- [x] **Environment provisioning wizard:** Dev/QA/Prod/demo/test with isolated configs - 2025.06.15 - AI: EnvironmentProvisioningService with wizard workflow and template-based setup
- [x] **First-launch diagnostic and compatibility check** - 2025.06.15 - AI: FirstLaunchDiagnosticService with comprehensive system analysis and compatibility validation
- [x] **Advanced rollback/restore on failed upgrade or migration** - 2025.06.15 - AI: AdvancedRollbackService with comprehensive backup, restore, rollback planning, upgrade monitoring, and system snapshots

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.3. UI/UX & Navigation
- [x] Visual wireframe/page builder - 2025.06.15 - AI: WireframePageBuilderService with comprehensive visual design, templating, code generation, and preview capabilities
- [x] Real-time translation manager, theme/color/logo editor
- [x] Accessibility compliance check
 - [x] **Per-role navigation structure and menu editing** - 2025-06-16 - AI: NavigationService filtrates menu by user role
 - [x] **Multi-level menu logic (mega menu, dynamic pop-out, favorites, search)** - 2025-06-20 - AI
- [x] Quick search field in NavMenu with live suggestions - 2025-06-17 - AI
 - [x] **Tenant/company custom menus and layout overrides** - 2025-06-16 - AI: NavigationService reads menuitems.{tenant}.json

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.4. Dashboard & Widgets
- [x] Dynamic widget marketplace (add, remove, drag, drop)
- [x] Realtime alerts, stats, plugin widget support - 2025-06-17 - AI: AlertWidget və plugin yüklənməsi
- [x] Custom dashboards per user/company/role
- [x] **Widget permission system (per module, per tenant, per user)**
- [x] **Widget dependency map and live error/highlight feedback** - 2025-06-17 - AI
- [x] **In-app widget store with usage analytics**

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.5. User, Role, Permission (Enterprise IAM)
- [x] Add/edit/remove users from UI
- [x] Role/permission matrix, fine-grained, team/group permission mapping
- [x] Unlimited custom permission types (UI, API, hardware, task, data, etc.)
- [x] Dynamic, conditional, temporary, time-scheduled permissions
- [x] **Delegated/just-in-time (JIT) permissions with expiry**
- [x] **Permission simulation, preview-as-other-user/role**
- [x] **Hierarchical group/OU-based (organizational units) role mapping**
- [x] **Per-module, per-action, per-field, per-feature toggle**

**For Developer qovluğu və Book:**
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.6. Reports, Audit, Export
- [x] Advanced filters, custom BI reporting
- [x] Export (PDF, Excel, CSV, JSON, Parquet)
- [x] Visual audit timeline, full audit event export (SIEM/Splunk/Syslog)
 - [x] Built-in notification center, alert templates, integrations (Email, Telegram, Slack)
- [x] **Audit log export/import, external SIEM/Syslog integration**
- [x] **Custom report designer, query builder, scheduled report delivery** - 2025-06-21 - AI
- [x] **Per-role/tenant reporting permissions**

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.7. Notification & Plugins
- [x] Modular notification engine, priority, channels, plugable provider
- [x] Plugin marketplace, versioning, config from UI
- [x] **Per-tenant and per-module notification config**
- [x] **Custom channels: SMS, push, Telegram, WhatsApp, webhooks**
- [x] **Bulk/broadcast and targeted notification logic**

**For Developer qovluğu və Book:**
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.8. Settings & Security
- [x] Settings UI for everything (theme, endpoints, auth)
- [x] Full audit of changes, instant rollback
- [x] 2FA, SSO, session, access logs, backup/restore from UI
- [x] Just-in-time privilege elevation, secret rotation
- [x] **Per-tenant/role security policy enforcement**
- [x] **Secret vault (hardware/TPM/HSM integration)**
- [x] **SAML, OIDC, OAuth2, LDAP integration**
- [x] **Auto-expiry, forced rotation, password policy enforcement**

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.9. Workflow & Form Editor
- [x] Visual workflow designer, form builder, validation rules
- [x] Workflow template marketplace, script automation - 2025-06-20 - AI: import/export, sharing və avtomatlaşdırma idarəsi əlavə edildi
- [x] **Multi-step, multi-actor approval**
- [x] **Script triggers (C#/Python/JS) per form/workflow**
- [x] **Real-time validation, condition-based logic**

**For Developer qovluğu və Book:**  
 Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb?
- [x] Nəyə xidmət edir?
- [x] İstifadə qaydası və idarəetmə prinsipləri
- [x] Texniki və biznes üstünlükləri
- [x] Gələcək inkişaf yolları və risklər
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.10. Test, Documentation, DevOps
- [x] Storybook for components, dev/QA/Prod profiles - 2025-06-21 - AI: komponent önizləmələri üçün Storybook quruldu
- [x] Changelog & documentation auto-generator, smart help - 2025-06-21 - AI: avtomatik changelog və sənədləşmə skriptləri əlavə edildi
- [x] Full test automation UI, auto-update handler - 2025-06-23 - AI: TestAutomationService və TestAutomation səhifəsi yaradıldı
- [x] **CI/CD pipeline integration, auto rollback on failed deploy** - 2025-06-21 - AI: ilkin GitHub Actions workflow və rollback skripti yaradıldı
  - 2025-06-22 - AI: Node `npm ci` and Storybook build added to CI/CD, static site artifact published
- [x] **Unit, integration, e2e, manual test checklist and evidence** - 2025-06-23 - AI: Docs/TestingChecklist.md sənədi əlavə olundu
- [x] **Developer portal and internal API docs auto-generation** - 2025-06-23 - AI: docfx ilə API sənədləri üçün generate-docs.sh skripti yeniləndi

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [x] Niyə yaradılıb? - 2025-06-23
- [x] Nəyə xidmət edir? - 2025-06-23
- [x] İstifadə qaydası və idarəetmə prinsipləri - 2025-06-23
- [x] Texniki və biznes üstünlükləri - 2025-06-23
- [x] Gələcək inkişaf yolları və risklər - 2025-06-23
- [x] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə) - 2025-06-23
- [x] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır - 2025-06-23
---

## 1.11. Multi-Database Support & Installation
- [x] Multi-DB wizard (SQL Server, PostgreSQL, SQLite, Oracle, NoSQL, etc.)
- [x] Auto-discover DB on network, manual entry, migrate/install
- [x] Real-time schema, backup/restore, live environment compatibility
- [x] Data warehouse integration, multi-source connector
- [x] **Per-tenant/per-region DB routing and failover**
- [x] **Automated DB schema upgrade, diff, and rollback**
- [x] **Data masking, DLP, and encryption-at-rest configuration**

## 1.12. Environment Configuration Manager
- [x] UI editor for env vars (API keys, DBs, URLs)
- [x] Per-environment profiles (Dev, QA, Prod, Demo)
- [x] Import/export configs (.json, .env), auto-sync, test before save
- [x] **Secrets encryption and audit for all sensitive configs**
- [x] **Config validation and dry-run test before apply**
- [x] **Role-based config visibility and change audit**

## 1.13. Live Process Monitor
- [x] Realtime backend/process tracker (CPU/RAM/Job state)
- [x] Kill/restart from UI, module/user/session filter, error auto-notify
- [x] **Historical process and resource usage analytics**
- [x] **Threshold alerting, auto-scale triggers**

## 1.14. Background Job Scheduler
- [x] Visual cron, job logs, per-job enable/disable, alert on failure/timeout
- [x] **Dependency chain, escalation, auto-retry**
- [x] **Job marketplace (import/export shared job definitions)**

## 1.15. Smart Update Manager
- [x] Version check, one-click update, backup-before-update, rollback on fail
- [x] **Per-tenant staged rollout, canary update logic**
- [x] **Patch management dashboard, update history log**

## 1.16. Support & Feedback
- [x] Ticket system, file/log attach, auto-context (user, screen, config), assignment
- [x] **AI assistant for support ticket suggestions**
- [x] **Feedback analytics, survey, NPS tracker**

## 1.17. UX Analytics & Heatmaps
- [x] Visual heatmap, session stats, click/usage filter/export
- [x] **Per-module, per-user, per-tenant analytics**
- [x] **Session replay and behavior pattern analysis**

## 1.18. Real-Time Device & User Monitor
- [x] List active client devices/sessions, online/offline tracker, live session/IP/module view
- [x] Auto-refresh, unauthorized access alert
- [x] **Device fingerprinting, anomaly/user risk scoring**
- [x] **Geofencing and session location analytics**

## 1.19. Modular Feature Toggle & Licensing
- [x] Feature toggle: All modules/tools/plugins (realtime/audio/video/chat/file) can be enabled/disabled from UI
- [x] License management, dynamic edition upgrades (Starter/Pro/Enterprise)
- [x] Per-license, per-feature, per-tenant visibility and enablement
- [x] **Feature flag management with usage analytics**
- [x] **Graceful degrade when feature/license is off/expired**

## 1.20. AI Assistant & Model Integration
- [x] **AI Model Hub:**
    - [x] OpenAI, Azure, HuggingFace, local LLM, Google Gemini, Meta, StabilityAI və s.
    - [x] API key/token management, dynamic provider switch
    - [x] Usage analytics (per user/tenant/feature)
    - [x] Audit log: who/when/which model/query/result
    - [x] AI in settings/logs/docs/feedback/BI/export
    - [x] Per-feature, per-license, per-user enable/disable
    - [x] “Shadow AI” (test new AI model w/o live impact)
    - [x] Prompt library, prompt versioning, role-based prompt publishing
    - [x] Explainable AI dashboard, fairness/bias metrics, human-in-the-loop
    - [x] AI plugin/module marketplace (upload, test, license, sell models)
    - [x] **Per-module AI toggle, per-sector use-case templates**
    - [x] **Data privacy guardrails for AI model usage**
    - [x] **Central audit and review panel for all AI-generated content**
---

## 1.21. Realtime Collaboration & Audit Trail
- [x] Show which admin is editing which settings/page
- [x] Lock/notify when a config is being edited by another user
- [x] Full history of every change + who/when
- [x] Revert individual fields or entire session

## 1.22. Dynamic Permission Conditions
- [x] Conditions per permission (e.g. only during work hours, IP range, user group)
- [x] Visual rule builder for dynamic access
- [x] Temporary permission elevation (with audit log)
- [x] Scheduled permission expiry

## 1.23. Unified API & Integration Hub
- [x] API key generator and revoke UI
- [x] Swagger/OpenAPI auto-doc viewer
- [x] Webhook configuration UI
- [x] Mobile app integration endpoint manager
- [x] Third-party plugin integration settings
- [x] Role-based API rate limiters
- [x] Ready-made connectors for Google Maps, Yandex Maps and similar 3rd party map APIs
- [x] Dynamic API integration wizard for external/partner SaaS or franchise systems
---

## 1.24. Multi-Tenant & SaaS Controls
- [x] Create, edit, delete tenants from UI
- [x] Assign users to tenants
- [x] Per-tenant database isolation or shared DB
- [x] Quota manager (storage, usage limits)
- [x] Tenant-level branding (logo/theme/configs)
- [x] Plan/Subscription system (Basic, Pro, etc.)
- [x] Franchise & branch management for franchise/federated models (future-ready)

## 1.25. Dynamic Company & Subcompany Management
- [x] Create unlimited companies, each fully isolated by data and config
- [x] Support company hierarchy (Company > Subcompany > Branch)
- [x] Assign users/devices/data per company or branch
- [x] Per-company branding (logo, color, theme, templates)
- [x] Company merge, split, or archive operations via UI
- [x] Tenant-aware API endpoints for true SaaS usage
- [x] Company-specific module/plugin toggles

## 1.26. Comprehensive Person & Identity Control
- [x] Manage people as Customer, User, Employee, Contractor, or Guest (convert anytime)
- [x] Assign multiple roles/profiles to same person (user+customer, employee+admin)
- [x] Multi-factor identity mapping (biometric, card, OTP, device binding)
- [x] View and manage contact, employment, credential, access logs from UI
- [x] Associate persons with multiple companies/branches/projects

## 1.27. Granular Dynamic Permission System
- [x] Unlimited permission types (UI button, API, DB, hardware, task, etc.)
- [x] Dynamic permission groups—create/edit/delete via UI
- [x] Per-module, per-page, per-component, and even per-field permission control
- [x] Permission inheritance, overrides, and conditional rules
- [x] Permission export/import as policy templates
---

## 1.28. Dynamic Module/Plugin Ecosystem
- [x] Install, update, enable/disable modules/plugins from UI (marketplace-like)
- [x] Per-company, per-user, or per-device plugin assignment
- [x] Plugin permission system (manage permissions by plugin and context)
- [x] Plugin sandboxing and health check status
- [x] Publish custom plugins to local/remote store from UI

## 1.29. Device & Brand Agnostic Integrations
- [x] Integrate/manage devices of any brand/model (IoT, biometric, sensors, POS…)
- [x] UI-driven device discovery, onboarding, grouping, monitoring
- [x] Device firmware update, health, alerting, usage history
- [x] Assign devices to companies, locations, people
- [x] Vendor/brand abstraction: auto-detect, generic API bridge
## 1.30. Data Lineage & Usage Governance
- [x] Visualize and track data origin, flow, usage (who/when/where/how used)
- [x] Full data export/import history and rollback
- [x] GDPR/Data sovereignty controls (per-company/user)
- [x] Custom data retention policies per company/user

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.31. Smart Alerts & Dynamic Automations
- [x] IFTTT-style automation builder (multi-condition, multi-action)
- [x] Trigger by any data, device, event, or time
- [x] Email, SMS, push, voice, hardware actions
- [x] Log, retry, escalate, and notify users/groups
- [x] Export/import alert/automation templates

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.32. Self-Service, Delegation & Approval Flows
- [x] Delegation of rights/roles with approval workflow
- [x] User self-service portal (profile, permissions, data, requests)
- [x] UI-based approval/rejection for critical ops
- [x] Notification/audit for all delegation/approval flows

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.33. System Health, Security & Audit Dashboard
- [x] Holistic health monitor (backend/frontend/DB/device/plugin)
- [x] Security audit (roles, permissions, auth, vulnerabilities, last scan)
- [x] Audit trail for every admin/user action
- [x] Instant remediation recommendations

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.34. Future-Proofing & Extensibility Tools
- [x] In-app module & DB migration advisor
- [x] Live code/plugin hot-reload (if permitted)
- [x] “Try new feature” toggle for beta modules
- [x] Roadmap, changelog, and deprecation alert viewer
- [x] Automated end-to-end integration/upgrade checklist

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.35. IP Telephony & Communication Systems Integration
- [x] Built-in connectors & UI management for 3CX, Asterisk, FreePBX, Teams, Cisco CallManager and similar IP phone systems
- [x] Call history, click-to-call, softphone panel, voicemail, real-time call analytics in dashboard
- [x] User/role-based telephony permission & number assignment matrix
- [x] Multi-channel comms bridge (SMS, fax, WhatsApp, Telegram, custom SIP)
- [x] All telephony features toggleable per company, user, module or session

## 1.36. Policy-as-Code (PaC) & Compliance
- [x] Visual Policy Editor (create/edit security, data, access policies)
- [x] Policy-as-Code: rules like "User X only allowed 9-18:00, AZ IPs"
- [x] Real-time policy validator/simulator
- [x] Compliance Dashboard (GDPR, ISO, SOC2, HIPAA, PCI…)

## 1.37. AI-Driven Predictive & Prescriptive Analytics
- [x] Predictive analytics for risk, usage, anomaly trends
- [x] AI-powered recommendations (disable old plugin, restrict risky user)
- [x] Auto root-cause analysis for errors (self-healing tips)

## 1.38. Fine-Grained Audit & Delegation Trails
- [x] Delegation chain visualization (who, what, when, to whom)
- [x] Just-in-time admin elevation (time-limit & auto-revert)
- [x] Full delegation/audit history (search/export)
---

## 1.39. Self-Documenting System & Smart Help
- [x] Auto-generate documentation, API, audit logs
- [x] AI-powered, context-aware doc/FAQ popup anywhere in UI

## 1.40. Dynamic Data Masking & Privacy Controls
- [x] Dynamic data masking (field/role/tenant-based)
- [x] Privacy mode toggle for sensitive data (affects export/log/monitoring)

## 1.41. Dynamic SLA (Service Level Agreement) Management
- [x] Visual SLA editor (uptime, response time, per tenant/user/device/module)
- [x] SLA breach alerts & dashboard

## 1.42. Distributed, Multi-Geo & Hybrid Cloud Control
- [x] Geo-fencing & region-based management (company/tenant/user/device)
- [x] Multi-cloud/hybrid management (AWS, Azure, GCP, On-prem)

## 1.43. Custom Branding, Portal White-label & Marketplace
- [x] White-label UI builder (theme, logo, login, branding per tenant/company)
- [x] In-app marketplace for modules/themes/plugins (buy/review/install)

## 1.44. Advanced Security & Threat Detection
- [x] Real-time threat intelligence feed
- [x] Attack simulation suite (phishing, privilege escalation, device spoofing…)
- [x] Session replay, tamper detection

## 1.45. Time Machine & Point-in-Time Restore
- [x] Snapshots & one-click restore for system/config/module/DB

## 1.46. Orchestration, Workflow & Automation Marketplace
- [x] Workflow template marketplace (import/export/clone workflows)
- [x] User script editor & sandbox (C#/Python/JS per event, UI trigger)

## 1.47. No-Code/Low-Code Module Builder
- [x] Visual module and page builder (add logic/UI/data sources without coding)
- [x] Marketplace for sharing no-code modules

## 1.48. Data Forensics, eDiscovery & Legal Hold
- [x] Enterprise audit, forensics export, legal hold/test mode
- [x] GDPR/CCPA data request, export, retention controls

## 1.49. Privacy Sandbox & Test Mode Everywhere
- [x] Per-module, per-UI, per-tenant test/demo mode
- [x] Safe test data purge, privacy sandbox for demo environments

## 1.50. Full OEM/White-label Partner Enablement
- [x] Custom partner/VAR branding, module packaging, licensing
- [x] Branded support, docs, help, “about” menu management

## 1.51. Edge & On-Premise Deployment Automation
- [x] SaaS + on-prem + edge auto-sync, update, failover, offline merge
---

## 1.52. Realtime Communication & Sharing (All Feature Toggleable)

Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı

- [ ] **Enable/disable in realtime:** Each function can be ON/OFF at any time from admin UI for ALL users/roles
- [ ] Realtime chat (user-to-user, group, multi-company)
- [ ] File sending (doc, image, video, arbitrary file types) with per-user, per-module toggle
- [ ] Audio/Video call (peer-to-peer & group) – SignalR/WebRTC engine (future-ready)
- [ ] Realtime screen sharing and remote control
- [ ] Per-user, per-role, per-module send/receive permission matrix
- [ ] Inline previews for received files/media
- [ ] Realtime reactions, mentions, push notifications
- [ ] Message moderation, logging and audit trail for compliance
- [ ] Disable/enable any part of communication system with one click
- [ ] Full session, channel, or company-level enable/disable for messaging, video, file

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.53. No-Code/Low-Code BI Studio & Data Warehouse Integration
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Visual BI report/chart designer, drag-and-drop dashboards - 2025-06-15 - AI: Sadə DashboardDesigner səhifəsi
- [ ] Data warehouse connector, multi-source query builder (SQL, NoSQL, API)
- [ ] Role-based BI sharing, dashboard marketplace

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.54. Native Mobile App Control & Notification Center
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Unified notification & task center for Android/iOS
- [ ] Push notification & in-app task assignments
- [ ] Mobile app marketplace management (approve/deny modules, features)

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.55. Cloud & Infrastructure Cost Optimization Studio
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Cloud provider integration (AWS, Azure, GCP, DigitalOcean, etc.) for billing/cost view
- [ ] Per-feature, per-module cost analytics and suggestions
- [ ] Proactive overage alerts, budget policies

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.56. Global Compliance & Data Residency Dashboard
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Track where data is stored/processed (per region, per customer, per module)
- [ ] Global compliance summary view (GDPR, CCPA, PCI DSS, SOC2, ISO 27001, HIPAA, etc.)
- [ ] Legal risk heatmaps and automated compliance task lists

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.57. Self-Healing & AI-Driven Infrastructure
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Auto-detect and self-repair infrastructure issues (AI-monitored recovery)
- [ ] Predictive scaling and hot/cold standby instance management (AI-driven)
- [ ] Admin notification and auto-rollforward/rollback on critical failures

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.58. Decentralized Identity & Web3 Support
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Integration with DID (Decentralized ID), blockchain-based auth
- [ ] Crypto wallet login, token-based permissions, NFT access
- [ ] Audit trail for blockchain transactions

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.59. Zero Trust Security Controls
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Policy-driven micro-segmentation (per session, per device, per resource)
- [ ] Continuous auth/re-auth, anomaly-based session closure
- [ ] Just-in-time privilege elevation, secret rotation, ephemeral tokens

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.60. Industry-Specific Compliance Modules
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] FedRAMP, FIPS 140-2, CJIS (US law), TISAX (automotive), EMVCo (fintech) support
- [ ] Real-time compliance certificate monitor, expiry, and renewal workflow

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.61. GreenOps & Sustainability Metrics
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Track energy use/carbon impact per feature, user, tenant, deployment
- [ ] Suggest cost and energy optimizations (AI-powered)
- [ ] “Sustainable Mode” — Auto-limits high-energy features on schedule

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.62. End-User Portal & Marketplace
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Customizable end-user self-service portal for customers/clients
- [ ] “Buy apps, add-ons, reports, modules” from app store
- [ ] Per-tenant customer/partner dashboards and service desk

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.63. LMS & Education Sector Integration
- [ ] Integration with LMS (Moodle, Canvas, Blackboard) for school/education use-cases
- [ ] Certification, grading, digital badge support in admin panel

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.63. LMS & Education Sector Integration
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Integration with LMS (Moodle, Canvas, Blackboard) for school/education use-cases
- [ ] Certification, grading, digital badge support in admin panel

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.64. AI Model Lifecycle Management
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Versioning, rollback, deployment, and usage analytics for custom-trained AI models
- [ ] “Shadow AI” testing (test new AI without end-user impact)
- [ ] Prompt library, prompt version history, role-based prompt deployment

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.65. Hardware Device Provisioning & Secure Enrollment
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Zero-touch IoT provisioning, QR/barcode-based device onboarding
- [ ] Remote wipe, lock, and secure device de-registration

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.66. Privacy & Data Sovereignty Automation
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] “Forget Me” workflows — instant data purge on user request (GDPR/CCPA)
- [ ] Dynamic data residency (per-user, per-region, per-tenant)

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.67. AI Ethics & Responsible AI Controls
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Explainable AI dashboards (show “why” a model made a decision)
- [ ] Fairness/bias analytics, human-in-the-loop approval workflows
- [ ] Dataset, prompt, and output audit logs (compliance with AI Act, etc.)

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.68. Modular ML/AI Marketplace
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Upload, test, license, or sell custom AI models/plugins from admin panel

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.69. B2B & Partner API Monetization
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] API monetization/billing controls, usage quotas, marketplace for third-party access

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## 1.70. Universal Audit Export
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Export all audit logs and business events in any format (JSON, CSV, Parquet) — integrate with SIEM/Splunk

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## ⏰ Dynamic Blocking & Access Control (User/Device/Tenant)
Gerçek @page route
Modern ve kullanıcı dostu arayüz
Simple/mock data
“Hazırlanır / Çox yaxında!” mesajı
Yazar/iletişim bilgileri
Menü entegrasyonu
Book güncelleme hatırlatıcısı
- [ ] Instantly block/unblock users, devices, tenants from UI
- [ ] **Time-based blocking:** Block for specified period (1hr, 3 days, custom)
- [ ] **Permanent blocking:** Until manually unblocked
- [ ] **Scheduled blocking/unblocking:** Set start/end (e.g., maintenance)
- [ ] Block by role, permission, IP, geo, device type, company, branch
- [ ] Instant unblock (from UI or automation)
- [ ] All block/unblock actions fully audited
- [ ] Auto-notify owner with block reason, unblock ETA
- [ ] Export block/unblock events for compliance/review

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## Bonus: Ultra-Granular Controls & UX
- [ ] Everything *searchable*, *filterable*, *exportable* (settings, roles, modules)
- [ ] Logs/configs/dashboards with drill-down, export (CSV/JSON/Excel/PDF), API access
- [ ] Undo/redo for all settings/configs
- [ ] Full multi-lingual UI/UX (add new language from UI)
- [ ] Customizable dashboards per user/company/role
- [ ] **Realtime session & connection explorer** (see all active users, sessions, devices, with drilldown)
- [ ] **Enterprise onboarding automation:** Create welcome kits, training, checklists, first login experiences
- [ ] **Proactive error reporting:** Users notified of possible errors before they happen (AI/monitoring)

**For Developer qovluğu və Book:**  
Bu modulda və ya alt modulda hər bir yeni funksiya, əlavə, düzəliş və ya texniki dəyişiklik olduqda, `For Developer` qovluğunda həmin modul üçün Book yenilənir:
- [ ] Niyə yaradılıb?
- [ ] Nəyə xidmət edir?
- [ ] İstifadə qaydası və idarəetmə prinsipləri
- [ ] Texniki və biznes üstünlükləri
- [ ] Gələcək inkişaf yolları və risklər
- [ ] İstifadəçi və developer üçün hər bir detal (hətta ən kiçik dəyişiklik belə)
- [ ] Sənəd **tam Azərbaycan dilində**, yalnız terminlər ingilis dilində saxlanılır
---

## ✅ Final QA Loop

- [ ] **Build the project**  
      _If any internet-based tools are required, download them._  
      _If Windows Terminal is needed, use it. You have full permissions — execute confidently._
- [ ] **Test the functionality**
- [ ] **Identify and document all errors**
- [ ] **Fix everything necessary**  
      _Without altering architecture or logic._
- [ ] **Build again**
- [ ] **Repeat until 100% error-free build and runtime is achieved**
- [ ] **Only then, mark the module as ✅ Completed and proceed to the next**

---

> **Qeyd:**  
> Bu panel istənilən ölçüdə layihə üçün (e-ticarət, taxi, məktəb, marketplace, legal, kargo, telekom, franchise, AI SaaS və s.)  
> tam universal, ultra-modular, açıq/kilitli, lisenziyaya görə fərqli görünüş və funksionallıq verə bilən,  
> gələcəkdə istənilən yeni texnologiya və biznes modelini problemsiz əlavə etməyə hazır ultra-enterprise idarəetmə mərkəzidir!


### 2. Manager Panel (WPF Desktop, EXE)

#### 2.1. Solution & Setup

- [ ] `/ManagerPanel/ManagerPanel.sln` – WPF .NET 8+, tray, branding, bütün ayarlar UI-dan

#### 2.2. Property & Tenant Management

- [ ] Mülk əlavə/çıxar/dəyiş, status, müqavilə
- [ ] Kirayəçi əlavə/çıxar, status, tarixçə

#### 2.3. Payment & Expense

- [ ] Ödənişlərin qəbul və qeydiyyatı (cash, POS, export)
- [ ] Xərclər, medaxil, aylıq/günlük hesabatlar

#### 2.4. Messaging, Feedback, Media

- [ ] Mesaj, cavab, media əlavə, feedback

#### 2.5. Reports & Analytics

- [ ] PDF/Excel çıxarış, qrafik, event log

#### 2.6. Notifications & Plugins

- [ ] Bildiriş tipi, plugin əlavə/çıxar, auto-update

#### 2.7. Test, Docs, Security

- [ ] Test checklist, diagnostics, user guide, password reset

---
##### ✅ Final QA Loop

- [ ] Build the project.
- [ ] Test the functionality.
- [ ] Identify and document all errors.
- [ ] Without changing the structure, working logic, or core principles, fix everything necessary.
- [ ] Build again.
- [ ] Repeat until 100% error-free build and runtime is achieved.
- [ ] Only after flawless build and operation, mark the module as completed and proceed.

---

### 3. SmartCustomer Mobile App (MAUI, iOS/Android)

#### 3.1. Solution & Setup

- [ ] `/SmartCustomerApp/SmartCustomerApp.sln` – .NET MAUI, visual onboarding, multi-language/theme

#### 3.2. Payment, Bills

- [ ] Ödənişlər, tarixçə, export, qəbz, cari borc/gəlir

#### 3.3. Komendant/Community Messaging

- [ ] Video/səs/şəkil göndərmə, subject/type seçimi, board/community paylaşımı

#### 3.4. Service Request

- [ ] Xidmət sifarişi, status izləmə, media əlavə

#### 3.5. Marketplace

- [ ] Elan yarat, media əlavə, board-da paylaş, şərh/voting

#### 3.6. Advanced Settings

- [ ] Bildiriş növü detallı, offline mode, QR/biometrik giriş, family/profil üzvü, feedback

#### 3.7. Security, Docs, Test

- [ ] Data encryption, session timeout, UI/manual test checklist, in-app FAQ

---
##### ✅ Final QA Loop

- [ ] Build the project.
- [ ] Test the functionality.
- [ ] Identify and document all errors.
- [ ] Without changing the structure, working logic, or core principles, fix everything necessary.
- [ ] Build again.
- [ ] Repeat until 100% error-free build and runtime is achieved.
- [ ] Only after flawless build and operation, mark the module as completed and proceed.

---

### 4. Reporting Desktop (WPF EXE)

#### 4.1. Solution & Setup

- [ ] `/ReportingDesktop/ReportingDesktop.sln`, self-contained exe, UI-dan bütün konfiqurasiya

#### 4.2. Reporting, Analytics, Export

- [ ] Dashboard, report list, filter, export, notification, plugin/extensions, auto-update

#### 4.3. User Guide, Docs, Security

- [ ] Test checklist, audit/log panel, feedback, in-app user guide

---
##### ✅ Final QA Loop

- [ ] Build the project.
- [ ] Test the functionality.
- [ ] Identify and document all errors.
- [ ] Without changing the structure, working logic, or core principles, fix everything necessary.
- [ ] Build again.
- [ ] Repeat until 100% error-free build and runtime is achieved.
- [ ] Only after flawless build and operation, mark the module as completed and proceed.

---

### 5. Reporting Mobile (MAUI/Flutter)

#### 5.1. Solution & Setup

- [ ] `/ReportingMobile/ReportingMobile.sln`, bütün ayar UI-dan, app icons, onboarding

#### 5.2. Reporting, Filters

- [ ] Dashboard, report list, filter, export, notification, offline/online

#### 5.3. Security, Test, Docs

- [ ] Data encryption, feedback, test checklist, user guide

---

### 6. Universal Enterprise Features

- [ ] Plugin/extensions, in-app update, workflow designer, dynamic form builder, cloud sync, backup, access matrix, key/token rotation, analytics, voice command, error tracking, SLA/contract, və s.

---
##### ✅ Final QA Loop

- [ ] Build the project.
- [ ] Test the functionality.
- [ ] Identify and document all errors.
- [ ] Without changing the structure, working logic, or core principles, fix everything necessary.
- [ ] Build again.
- [ ] Repeat until 100% error-free build and runtime is achieved.
- [ ] Only after flawless build and operation, mark the module as completed and proceed.

---
