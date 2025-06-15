# Frontend_TODO.md

---

## CONTACT

| Language | Contact Info |
|:--|:--|
| **EN** | **ASL.LivingGrid** frontend modules authored and maintained by: Vusal Azer oglu Mastaliyev.<br> Phone: +994513331383 <br> Email: mr.lasuv@gmail.com <br> Role: Lead Developer, Enterprise Architect, UI/UX Owner, Solution Lead. |

---

## ENTERPRISE FRONTEND RULES

- Every module (WebAdmin, Manager, SmartCustomer, ReportingDesktop, ReportingMobile) is a 100% independent Visual Studio solution.
- All configs, endpoints, settings, language, notifications, and database options are managed via UI—nothing hardcoded.
- Multi-language UI, all data and interface elements, notifications, and documentation must support: AZ, EN, TR, RU.
- All backend API, SignalR/gRPC, database installation, reporting, plugin, workflow, and AI endpoints must be consumed via configurable UI.
- Every frontend must enable dynamic integration with multiple backend database types (SQL Server, PostgreSQL, SQLite) via API and allow installation/setup from UI.
- Real-time and background events, notifications, and reporting must be fully interactive and visual.
- Every step completed must be checked `[x]`; only continue from the first unchecked `[ ]` step; nothing skipped or repeated.

---

## CHECKLIST USAGE

1. Mark every finished task with `[x]` immediately.
2. Work only on the first unchecked `[ ]` step.
3. Log date and person (or AI) for each completed step (optional, but recommended).
4. When all tasks are checked, mark the section as Done and move to the next.
5. All integration, logging, error, status, and audit flows must be fully UI-driven.

---

## FRONTEND TODO – DETAILED CHECKLIST

---

### 1. WebAdmin Panel (Blazor, Kestrel, EXE)

#### 1.1. Solution & Setup

- [ ] `/WebAdminPanel/WebAdminPanel.sln` as an independent solution
- [ ] Blazor Server/WASM scaffolding with Kestrel self-hosted .exe
- [ ] Tray icon, minimize-to-tray, open browser on start
- [ ] All settings/configs (including API, database, network) from UI only

#### 1.2. Multi-Database Support & Dynamic Installation

- [ ] UI wizard for “Database Setup/Discovery/Install”
- [ ] Auto-discover all SQL Server/PostgreSQL servers on network (API call)
- [ ] Manual entry for DB server/IP/port/credentials
- [ ] Trigger automated database install/setup to chosen server via backend API
- [ ] Progress, status, errors, and success messages are always visual (not hidden)
- [ ] Ability to choose database type (SQL Server, PostgreSQL, SQLite), and switch/reconfigure anytime
- [ ] Show schema, version, migration, upgrade, and rollback status
- [ ] Install sample/demo data for first-time usage

#### 1.3. UI/UX & Navigation

- [ ] Wireframes and final design for every screen and state (including DB setup, notification, plugin, dashboard, messaging, reports)
- [ ] Drag/drop, multi-tab navigation, adaptive mobile/responsive
- [ ] Multi-language switch (AZ/EN/TR/RU) in UI, with live translation editor
- [ ] Theming, color, logo, and font customization from settings
- [ ] Accessibility mode and checker

#### 1.4. Real-Time & Notification

- [ ] SignalR client for all real-time events (notification, chat, reporting, dashboard, plugins)
- [ ] Display real-time messages, warnings, progress, status, and errors
- [ ] Notification preferences (per-user, per-module, silent modes)

#### 1.5. User, Role, Security

- [ ] User management UI: create/edit/remove user, roles, and teams
- [ ] 2FA, password reset, login history, device management
- [ ] Permission matrix and security settings (UI-managed)

#### 1.6. Reporting & Analytics

- [ ] Reports dashboard (daily/monthly/advanced/custom)
- [ ] Filters, export (PDF/Excel/CSV), dynamic report builder
- [ ] Real-time data, widget/chart integration (with SignalR)
- [ ] Visual audit/history of all actions, changes, and reports

#### 1.7. Plugins, Marketplace & Extensibility

- [ ] UI plugin/module manager (list, install, update, remove plugins)
- [ ] Widget/plugin gallery, one-click install, visual config
- [ ] Marketplace integration (download/purchase/install third-party modules)
- [ ] Feature toggles and per-tenant plugin enable/disable

#### 1.8. Workflow & Form Builder

- [ ] Visual workflow builder, process designer, and approval chains
- [ ] Dynamic form builder (add, edit, remove fields, UI preview)
- [ ] Save/publish workflows/forms with real-time update and rollback

#### 1.9. AI & Automation

- [ ] AI settings (API key, endpoint selection) and quota management from UI
- [ ] UI for AI-driven chat, suggestions, reporting, feedback, process automation
- [ ] Visual feedback for AI actions, errors, logs, and output

#### 1.10. Advanced Logging, Audit, Troubleshooting

- [ ] Full UI log/audit viewer for:  
    - [ ] User actions  
    - [ ] API/backend events  
    - [ ] DB install/upgrade/migration  
    - [ ] Real-time errors and status  
    - [ ] Notification/alert history  
    - [ ] Security, permission, and configuration changes  
- [ ] Download/export logs for admin review (GDPR-compliant)
- [ ] Custom log search/filter, date range, category

#### 1.11. Test, Documentation & DevOps

- [ ] Integrated UI test checklist, documentation browser
- [ ] Change log viewer, auto-update status
- [ ] Links to API documentation, business logic, admin/dev user guides

---

### 2. Manager Panel (WPF Desktop, EXE)

(Same pattern: solution setup, user/tenant/property, payment, expense, reporting, notification, plugin, AI, DB config, logs, audit...)

---

### 3. SmartCustomer Mobile App (MAUI, iOS/Android)

(Same pattern: solution setup, onboarding, payment, community messaging, service request, AI, real-time, notification, report, feedback, plugin, DB config...)

---

### 4. Reporting Desktop (WPF EXE) & 5. Reporting Mobile (MAUI/Flutter)

(Same: setup, dashboard/report/filter/export, notification, plugin, real-time, AI, logs, DB config...)

---

## UNIVERSAL ENTERPRISE FEATURES

- [ ] Per-module, per-tenant, per-user settings for everything—always via UI
- [ ] Multi-database discovery, install, migration, backup/restore, and switching—full UI wizard, always tracked and logged
- [ ] Real-time SignalR, WebSocket, and gRPC support for all relevant modules
- [ ] AI and automation UI for reporting, feedback, messaging, and workflow
- [ ] Marketplace and plugin extensibility
- [ ] Enterprise-grade error logging, audit, monitoring—fully visual and filterable

---

## ARCHIVE

- [ ] (Completed tasks and lessons learned will be archived here)

---

**Always continue from the first unchecked `[ ]` step—nothing skipped, nothing repeated!  
All features, including dynamic DB, install, reporting, AI, real-time, plugin, and logging, are covered and tracked.**
