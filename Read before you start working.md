# Read before you start working

**Always read this file before you start any development or review session. Make sure you are aligned with all the principles below and the current progress status for each section.**
Always use .NET 9.0
ASL in each project case. let it be
The project name is ASL.LivingGrid
Each project should be independent and have its own .sln, but it should also be in the .sln that covers the entire project
---


## 1. Mandatory Core Rules

- Always check if this file exists. If not, create it using the default template below.
- Always learn, re-learn, and teach back: AI must check, read, and summarize this document before starting any step.
- On every session start, AI must read the current status and pending items from here, and only then proceed.
- At each phase, document what has been done and what needs to be done in this file.
- **Core Rules and Workflow:**  
    - All rule tracking, conventions, and workflow updates are in `CoreRules_TODO.md`.
    - Always check and update `CoreRules_TODO.md` before any project work.  
    - Only when the Core Rules status is **“1 - Done”** in the progress table below, you may move to other sections.
    - If this file or `CoreRules_TODO.md` does not exist, create them from the default template and follow the initialization process.
- **Status for each main section:**  
    1 - Done  
    2 - Working on it  
    3 - To be reviewed later
- Only work on the section that is currently **“2 - Working on it”** in the progress table below.

---
## 2. Front End

- UI/UX, components, client logic, and integration.
- All tasks, progress, and sub-tasks are tracked in a separate file: `Frontend_TODO.md`.

---

## 3. Back End

- API, database, business logic, background jobs.
- All tasks, progress, and sub-tasks are tracked in a separate file: `Backend_TODO.md`.

---

## 4. Comprehensive Testing & Final Review

- End-to-end testing, automation, code review, documentation, and user acceptance.
- All related tasks and progress are tracked in a separate file: `Testing_TODO.md`.

---

## Progress Table

| Section              | Status (1-Done, 2-Working on it, 3-To be reviewed later) |
|----------------------|----------------------------------------------------------|
| Core Rules           | 1 - Done                                                 |
| CoreRules_TODO.md    | 1 - Done                                                 |
| Front End            | 2 - Working on it                                        |
| Back End             | 3 - To be reviewed later                                 |
| Testing & Review     | 3 - To be reviewed later                                 |

---

**Rule:**  
*Before starting any work, always check this table and only process the section marked as “2 - Working on it”. When you finish all items in that section’s TODO file, mark it as “1 - Done” here and set the next section’s status to “2 - Working on it”. Always log and check progress before you code! The section “CoreRules_TODO.md” is the master control for rules and workflow, and must always be up-to-date before moving forward.*
