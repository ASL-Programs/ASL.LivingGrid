# LocalizationBook

Bu kitab WebAdminPanel modulunda tərcümə idarəçiliyi və dil paketlərinin yaradılması qaydalarını izah edir.

## Əsas Funksiyalar
- Dil resurslarının toplu redaktəsi
- Versiya tarixi və təsdiqləmə mexanizmi
- JSON formatında import və export
- REST API vasitəsilə dil paketlərinin idarəsi

## İstifadə Qaydası
1. `/languagepacks` səhifəsinə keçid edərək mövcud dillərin siyahısını görün.
2. Dil seçib "Export" düyməsi ilə mövcud tərcümələri JSON fayl kimi yükləyin.
3. Eyni formatda faylı seçib yükləyərək `Import` əməliyyatı aparın.
4. API vasitəsilə `/api/localization` endpoint-lərindən də istifadə etmək mümkündür.

Bu sənəd daim yenilənəcək.
