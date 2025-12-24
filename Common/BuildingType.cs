namespace Common;

public enum BuildingType
{
    // Производители (класс 1)
    Logging,          // Лесозаготовка
    Quarry,           // Карьер
    Mine,             // Шахта
    Farm,             // Поле
    
    // Переработчики (класс 2)
    Sawmill,          // Лесопилка
    KilnFurnace,      // Печь обжига
    Smelter,          // Плавильня
    Charcoal,         // Углежог
    Crusher,          // Дробилка
    Bakery,           // Пекарня
    
    // Переработчики (класс 3)
    Carpentry,        // Столярная
    Stonemason,       // Каменотёс
    Forge,            // Кузница
    Glassworks,       // Стекловарня
    Armory,           // Оружейная
    
    // Драгоценности (класс 4)
    Laboratory,       // Лаборатория
    AlchemicalFurnace,// Алхимическая печь
    
    // Потребители
    Barracks,         // Казармы
    
    // Оборона
    Barricade,        // Баррикада
    DefenseTower      // Оборонительная башня
}
