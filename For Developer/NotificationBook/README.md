# NotificationBook

Bu sənəd WebAdminPanel modulunda bildiriş mərkəzinin və Slack kanalının istifadəsini izah edir.

## Qısa xülasə
Bildiriş xidməti müxtəlif kanallar vasitəsilə məlumat göndərməyə imkan verir. Yeni **SlackNotificationChannel** `Notifications:SlackWebhookUrl` ünvanına POST sorğusu edərək mesajları Slack-də göstərir.

## Niyə yaradılıb?
- İstifadəçiləri operativ şəkildə xəbərlərdən xəbərdar etmək.
- Slack kimi məşhur əməkdaşlıq platformasına inteqrasiyanı asanlaşdırmaq.

## Nəyə xidmət edir?
- E-poçt, SMS, Telegram və Webhook kanallarına əlavə olaraq Slack üzərindən də bildiriş göndərməyə.
- `/notifications-center` səhifəsində istifadəçi üçün toplanmış bildirişləri oxumağa və idarə etməyə.

## İstifadə qaydası və idarəetmə prinsipləri
1. `appsettings.json` faylında `Notifications:EnableSlackNotifications` dəyərini `true` edin və `Notifications:SlackWebhookUrl` sahəsinə Slack webhook ünvanını yazın.
2. Sistem daxilində bildiriş yaradıldıqda `SlackNotificationChannel` avtomatik həmin webhook-a mesaj göndərəcək.
3. İstifadəçi menyusundan **Bildiriş Mərkəzi** bölməsinə daxil olaraq bütün bildirişləri görə və "Read" düyməsi ilə oxundu kimi işarələyə bilər.

## Texniki və biznes üstünlükləri
- Komandadaxili operativlik artır, çünki kritik məlumatlar Slack kanallarında dərhal görünür.
- Sadə konfiqurasiya ilə istənilən şirkət və tenant üçün aktivləşdirilə bilər.

## Gələcək inkişaf yolları və risklər
- Mesaj formatının zənginləşdirilməsi (emoji, bloklar, linklər).
- Webhook ünvanının yanlış qurulması mesajların itməsi riskini yaradır.
