using BepInEx;
using HarmonyLib;
using Ostranauts.Core.Tutorials;
using Ostranauts.InputControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OstranautsRuTranslation
{
    [BepInPlugin("ru.skobochki.rutranslation", "ostranautsRu", "0.4.0")]
    public class RuTranslation : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("[RU] Ru Translation Set Up");
            try
            {
                var _ = GrammarUtils.inflectedStrings;
                Logger.LogInfo("[RU] GrammarUtils initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[RU] Failed to initialize GrammarUtils: {ex.Message}");
            }

            ReplaceGrammarDictionaries();

            Harmony harmony = new Harmony("ru.skobochki.rutranslation");
            harmony.PatchAll();
        }


        private void ReplaceGrammarDictionaries()
        {
            Type grammarType = typeof(GrammarUtils);

            var allStaticFields = grammarType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo partsField = null;
            FieldInfo partsSentenceField = null;
            var targetDictType = typeof(Dictionary<GrammarUtils.GrammarLUTIndex, string[]>);

            foreach (var field in allStaticFields)
            {
                if (field.FieldType == targetDictType)
                {
                    if (partsField == null) partsField = field;
                    else partsSentenceField = field;
                }
            }

            if (partsField == null || partsSentenceField == null)
            {
                Logger.LogError("[RU] Could not locate the two dictionary fields.");
                return;
            }

            Logger.LogInfo($"[RU] Found fields: '{partsField.Name}' and '{partsSentenceField.Name}'");
            
            var russianParts = new Dictionary<GrammarUtils.GrammarLUTIndex, string[]>
    {
        { GrammarUtils.GrammarLUTIndex.Subjective, new[] { "я", "ты", "он", "она", "они", "оно" } },
        { GrammarUtils.GrammarLUTIndex.Possessive, new[] { "мой", "твой", "его", "её", "их", "его" } },
        { GrammarUtils.GrammarLUTIndex.Objective, new[] { "меня", "тебя", "его", "её", "их", "его" } },
        { GrammarUtils.GrammarLUTIndex.Reflexive, new[] { "себя", "себя", "себя", "себя", "себя", "себя" } },
        { GrammarUtils.GrammarLUTIndex.ContractIs, new[] { "я", "ты", "он", "она", "они", "оно" } },
        { GrammarUtils.GrammarLUTIndex.ContractHas, new[] { "я имею", "ты имеешь", "он имеет", "она имеет", "они имеют", "оно имеет" } },
        { GrammarUtils.GrammarLUTIndex.ContractWill, new[] { "я буду", "ты будешь", "он будет", "она будет", "они будут", "оно будет" } }
    };

            var russianPartsSentence = new Dictionary<GrammarUtils.GrammarLUTIndex, string[]>
    {
        { GrammarUtils.GrammarLUTIndex.Subjective, new[] { "Я", "Ты", "Он", "Она", "Они", "Оно" } },
        { GrammarUtils.GrammarLUTIndex.Possessive, new[] { "Мой", "Твой", "Его", "Её", "Их", "Его" } },
        { GrammarUtils.GrammarLUTIndex.Objective, new[] { "Меня", "Тебя", "Его", "Её", "Их", "Его" } },
        { GrammarUtils.GrammarLUTIndex.Reflexive, new[] { "Себя", "Себя", "Себя", "Себя", "Себя", "Себя" } },
        { GrammarUtils.GrammarLUTIndex.ContractIs, new[] { "Я", "Ты", "Он", "Она", "Они", "Оно" } },
        { GrammarUtils.GrammarLUTIndex.ContractHas, new[] { "Я имею", "Ты имеешь", "Он имеет", "Она имеет", "Они имеют", "Оно имеет" } },
        { GrammarUtils.GrammarLUTIndex.ContractWill, new[] { "Я буду", "Ты будешь", "Он будет", "Она будет", "Они будут", "Оно будет" } }
    };

            partsField.SetValue(null, russianParts);
            partsSentenceField.SetValue(null, russianPartsSentence);

            Logger.LogInfo("[RU] Grammar dictionaries successfully replaced with Russian pronouns.");
        }
    }

    // Tutorial Section

    // Patch For BearingShow

    [HarmonyPatch(typeof(BearingShow), "get_ObjectiveName")]
    public static class Patch_BearingShow_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Наведитесь на OKLG";
            return false;
        }
    }

    [HarmonyPatch(typeof(BearingShow), "get_ObjectiveDesc")]
    public static class Patch_BearingShow_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Используйте элементы управления вращением, пока корабль не будет направлен в сторону OKLG (BRG около 0, цель — OKLG).";
            return false;
        }
    }

    [HarmonyPatch(typeof(BearingShow), "get_ObjectiveDescComplete")]
    public static class Patch_BearingShow_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Направлено на OKLG.";
            return false;
        }
    }

    // Patch For CalibrateCCW

    [HarmonyPatch(typeof(CalibrateCCW), "get_ObjectiveName")]
    public static class Patch_CalibrateCCW_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибруйте тягу против часовой стрелки";
            return false;
        }
    }

    [HarmonyPatch(typeof(CalibrateCCW), "get_ObjectiveDesc")]
    public static class Patch_CalibrateCCW_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажимайте на кнопку \"" + InputManager.GetGlyphString("Turn CCW", null) + "\" пока не начнётесь вращаться против часовой стрелки.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CalibrateCCW), "get_ObjectiveDescComplete")]
    public static class Patch_CalibrateCCW_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибрована тяга против часовой стрелки";
            return false;
        }
    }


    // Patch For CalibrateCW
    [HarmonyPatch(typeof(CalibrateCW), "get_ObjectiveName")]
    public static class Patch_CalibrateCW_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибруйте тягу по часовой стрелке";
            return false;
        }
    }

    [HarmonyPatch(typeof(CalibrateCW), "get_ObjectiveDesc")]
    public static class Patch_CalibrateCW_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажимайте на кнопку \"" + InputManager.GetGlyphString("Turn CW", null) + "\" чтобы повернуть свой корабль по часовой стрелке.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CalibrateCW), "get_ObjectiveDescComplete")]
    public static class Patch_CalibrateCW_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Тяга по часовой стрелке откалибрована";
            return false;
        }
    }


    // Patch For CollectEquipment

    [HarmonyPatch(typeof(CollectEquipment), "get_ObjectiveName")]
    public static class Patch_CollectEquipment_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Заберите костюм и шлем";
            return false;
        }
    }

    [HarmonyPatch(typeof(CollectEquipment), "get_ObjectiveDesc")]
    public static class Patch_CollectEquipment_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откройте инвентарь на ближайшей стойке. Переместите сумку в слот для вашей сумки, затем поместите костюм и шлем в слот инвентаря.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CollectEquipment), "get_ObjectiveDescComplete")]
    public static class Patch_CollectEquipment_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сумка, костюм, шлем — всё в наличии и исправно.";
            return false;
        }
    }


    // Patch For CrowbarHallway2
    [HarmonyPatch(typeof(CrowbarHallway2), "get_ObjectiveName")]
    public static class Patch_CrowbarHallway2_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Исследовать станцию";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway2), "get_ObjectiveDesc")]
    public static class Patch_CrowbarHallway2_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Это не прямой путь к вашему кораблю. Но иногда отклонение от проторенной дороги приносит свои плоды.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway2), "get_ObjectiveDescComplete")]
    public static class Patch_CrowbarHallway2_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Хм. Что это за переключатель?";
            return false;
        }
    }



    // Patch For CrowbarHallway3
    [HarmonyPatch(typeof(CrowbarHallway3), "get_ObjectiveName")]
    public static class Patch_CrowbarHallway3_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Проверьте выключатель в коридоре";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway3), "get_ObjectiveDesc")]
    public static class Patch_CrowbarHallway3_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Попробуйте переключить выключатель в конце коридора.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway3), "get_ObjectiveDescComplete")]
    public static class Patch_CrowbarHallway3_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Любопытство удовлетворено.";
            return false;
        }
    }



    // Patch For CrowbarHallway4
    [HarmonyPatch(typeof(CrowbarHallway4), "get_ObjectiveName")]
    public static class Patch_CrowbarHallway4_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Исследуйте дальше";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway4), "get_ObjectiveDesc")]
    public static class Patch_CrowbarHallway4_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Теперь дверь под напряжением, заберите свою награду на другой стороне.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway4), "get_ObjectiveDescComplete")]
    public static class Patch_CrowbarHallway4_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Наконец-то есть чем похвастаться.";
            return false;
        }
    }


    // Patch For CrowbarHallway5
    [HarmonyPatch(typeof(CrowbarHallway5), "get_ObjectiveName")]
    public static class Patch_CrowbarHallway5_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Взломайте дверь";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway5), "get_ObjectiveDesc")]
    public static class Patch_CrowbarHallway5_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Там есть обесточенная дверь, ведущая обратно в остальную часть K-LEG. Взломайте её с помощью лома.";
            return false;
        }
    }

    [HarmonyPatch(typeof(CrowbarHallway5), "get_ObjectiveDescComplete")]
    public static class Patch_CrowbarHallway5_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Гражданский долг выполнен.";
            return false;
        }
    }


    // Patch For DerelictComms
    [HarmonyPatch(typeof(DerelictComms), "get_ObjectiveName")]
    public static class Patch_DerelictComms_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключитесь на связь (снова)";
            return false;
        }
    }

    // Patch For DismissNote
    [HarmonyPatch(typeof(DismissNote), "get_ObjectiveName")]
    public static class Patch_DismissNote_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отклонить заметку";
            return false;
        }
    }

    [HarmonyPatch(typeof(DismissNote), "get_ObjectiveDesc")]
    public static class Patch_DismissNote_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите на бумажную шпаргалку, чтобы убрать её.";
            return false;
        }
    }

    [HarmonyPatch(typeof(DismissNote), "get_ObjectiveDescComplete")]
    public static class Patch_DismissNote_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Шпаргалка отклонена";
            return false;
        }
    }


    // Patch For DmgVizOff
    [HarmonyPatch(typeof(DmgVizOff), "get_ObjectiveName")]
    public static class Patch_DmgVizOff_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Скрыть информацию об износе и повреждениях";
            return false;
        }
    }

    [HarmonyPatch(typeof(DmgVizOff), "get_ObjectiveDesc")]
    public static class Patch_DmgVizOff_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите '" + InputManager.GetGlyphString("Toggle PDA Power Vizor", null) + "', а затем кнопку DEFAULT, чтобы снова скрыть наложение износа и повреждений.";
            return false;
        }
    }

    [HarmonyPatch(typeof(DmgVizOff), "get_ObjectiveDescComplete")]
    public static class Patch_DmgVizOff_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Намного лучше.";
            return false;
        }
    }


    // Patch For DmgVizShow
    [HarmonyPatch(typeof(DmgVizShow), "get_ObjectiveName")]
    public static class Patch_DmgVizShow_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Включить отображение износа и повреждений";
            return false;
        }
    }

    [HarmonyPatch(typeof(DmgVizShow), "get_ObjectiveDesc")]
    public static class Patch_DmgVizShow_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите '" + InputManager.GetGlyphString("Toggle PDA Power Vizor", null) + "', а затем кнопку DAMAGE, чтобы увидеть, насколько изношены или повреждены предметы.";
            return false;
        }
    }

    [HarmonyPatch(typeof(DmgVizShow), "get_ObjectiveDescComplete")]
    public static class Patch_DmgVizShow_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Износ приводит к поломке предметов.";
            return false;
        }
    }

    // Patch For DockWithDerelict
    [HarmonyPatch(typeof(DockWithDerelict), "get_ObjectiveName")]
    public static class Patch_DockWithDerelict_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Состыковаться с заброшенным кораблём";
            return false;
        }
    }

    [HarmonyPatch(typeof(DockWithDerelict), "get_ObjectiveDesc")]
    public static class Patch_DockWithDerelict_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Используйте те же элементы управления полётом, чтобы приблизиться к стыковочному кольцу со скоростью менее 100 м/с VREL, RNG около нуля, выровняйте стыковочные кольца, затем нажмите CLAMPS, когда индикатор \"CLAMP ALIGN\" загорится зелёным.";
            return false;
        }
    }

    [HarmonyPatch(typeof(DockWithDerelict), "get_ObjectiveDescComplete")]
    public static class Patch_DockWithDerelict_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Успешная стыковка.";
            return false;
        }
    }

    // Patch For ExpandMTT
    [HarmonyPatch(typeof(ExpandMTT), "get_ObjectiveName")]
    public static class Patch_ExpandMTT_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Развернуть мега-подсказку";
            return false;
        }
    }

    [HarmonyPatch(typeof(ExpandMTT), "get_ObjectiveDesc")]
    public static class Patch_ExpandMTT_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите 'Показать ещё', чтобы просмотреть дополнительную информацию о выбранном предмете, например физические свойства, а в случае людей — их эмоциональное состояние.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ExpandMTT), "get_ObjectiveDescComplete")]
    public static class Patch_ExpandMTT_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Мега-подсказка развёрнута.";
            return false;
        }
    }


    // Patch For ExploreTutorialDerelict
    [HarmonyPatch(typeof(ExploreTutorialDerelict), "get_ObjectiveName")]
    public static class Patch_ExploreTutorialDerelict_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Исследовать заброшенный корабль";
            return false;
        }
    }

    [HarmonyPatch(typeof(ExploreTutorialDerelict), "get_ObjectiveDesc")]
    public static class Patch_ExploreTutorialDerelict_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Возьмите ручной фонарь в руку, включите его. Зайдите внутрь заброшенного корабля.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ExploreTutorialDerelict), "get_ObjectiveDescComplete")]
    public static class Patch_ExploreTutorialDerelict_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Первый заброшенный корабль посещён.";
            return false;
        }
    }


    // Patch For ForwardThrust
    [HarmonyPatch(typeof(ForwardThrust), "get_ObjectiveName")]
    public static class Patch_ForwardThrust_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибровать тягу вперёд";
            return false;
        }
    }

    [HarmonyPatch(typeof(ForwardThrust), "get_ObjectiveDesc")]
    public static class Patch_ForwardThrust_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите клавишу \"" + InputManager.GetGlyphString("Thrust Up", null) + "\", чтобы снова снизить VREL ниже 10 м/с (с целевым OKLG).";
            return false;
        }
    }

    [HarmonyPatch(typeof(ForwardThrust), "get_ObjectiveDescComplete")]
    public static class Patch_ForwardThrust_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Тяга вперёд откалибрована.";
            return false;
        }
    }


    // Patch For GainClearance
    [HarmonyPatch(typeof(GainClearance), "get_ObjectiveName")]
    public static class Patch_GainClearance_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Получить разрешение на стыковку";
            return false;
        }
    }

    [HarmonyPatch(typeof(GainClearance), "get_ObjectiveDesc")]
    public static class Patch_GainClearance_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Выберите \"Вызов корабля\", выберите ближайший \"?\", и инициируйте процедуру стыковки с заброшенным кораблём.";
            return false;
        }
    }

    [HarmonyPatch(typeof(GainClearance), "get_ObjectiveDescComplete")]
    public static class Patch_GainClearance_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Разрешение на стыковку получено.";
            return false;
        }
    }





    // Patch For GetDressed

    [HarmonyPatch(typeof(GetDressed), "get_ObjectiveName")]
    public static class Patch_GetDressed_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Оденьтесь";
            return false;
        }
    }

    [HarmonyPatch(typeof(GetDressed), "get_ObjectiveDesc")]
    public static class Patch_GetDressed_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переместите жёлтый комбинезон и обувь с пола на вашего персонажа в окне инвентаря.";
            return false;
        }
    }

    [HarmonyPatch(typeof(GetDressed), "get_ObjectiveDescComplete")]
    public static class Patch_GetDressed_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Одеты и готовы.";
            return false;
        }
    }


    // Patch For HailShip
    [HarmonyPatch(typeof(HailShip), "get_ObjectiveName")]
    public static class Patch_HailShip_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вызвать корабль";
            return false;
        }
    }

    [HarmonyPatch(typeof(HailShip), "get_ObjectiveDesc")]
    public static class Patch_HailShip_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите кнопку \"Вызов корабля\" в правой части консоли связи. Выберите \"OKLG\" из списка контактов.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HailShip), "get_ObjectiveDescComplete")]
    public static class Patch_HailShip_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Канал связи с OKLG открыт.";
            return false;
        }
    }


    // Patch For HallwayConduit2
    [HarmonyPatch(typeof(HallwayConduit2), "get_ObjectiveName")]
    public static class Patch_HallwayConduit2_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Ослабленный кабель-канал";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit2), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit2_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "В коридоре есть незакреплённый кабель-канал. Переустановите его для кармической награды. Начните с нажатия " + InputManager.GetGlyphString("RightClick", null) + " на нём.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit2), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit2_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Кабель-канал выбран.";
            return false;
        }
    }


    // Patch For HallwayConduit3
    [HarmonyPatch(typeof(HallwayConduit3), "get_ObjectiveName")]
    public static class Patch_HallwayConduit3_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переустановить кабель-канал";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit3), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit3_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Выберите действие 'Установить' на панели быстрых действий.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit3), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit3_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Режим размещения активирован.";
            return false;
        }
    }


    // Patch For HallwayConduit4
    [HarmonyPatch(typeof(HallwayConduit4), "get_ObjectiveName")]
    public static class Patch_HallwayConduit4_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Позиционируйте временный кабель-канал";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit4), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit4_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Наложите временный кабель-канал на предназначенное место, затем нажмите " + InputManager.GetGlyphString("Click", null) + ", чтобы начать установку.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit4), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit4_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Установка начата.";
            return false;
        }
    }


    // Patch For HallwayConduit5
    [HarmonyPatch(typeof(HallwayConduit5), "get_ObjectiveName")]
    public static class Patch_HallwayConduit5_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Завершить установку";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit5), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit5_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Установка требует времени. Помните, что вы можете ускорить время при выполнении длительных задач.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit5), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit5_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Время ускорено.";
            return false;
        }
    }

    // Patch For HallwayConduit6
    [HarmonyPatch(typeof(HallwayConduit6), "get_ObjectiveName")]
    public static class Patch_HallwayConduit6_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Открыть дверь";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit6), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit6_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Установка кабель-канала восстановила питание двери. Обыщите комнату за ней, чтобы найти свою награду.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit6), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit6_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Порог пересечён.";
            return false;
        }
    }

    // Patch For HallwayConduit7
    [HarmonyPatch(typeof(HallwayConduit7), "get_ObjectiveName")]
    public static class Patch_HallwayConduit7_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Обыскать стойку";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit7), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit7_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Это где-то в темноте...";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit7), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit7_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Никтофобия преодолена.";
            return false;
        }
    }

    // Patch For HallwayConduit8
    [HarmonyPatch(typeof(HallwayConduit8), "get_ObjectiveName")]
    public static class Patch_HallwayConduit8_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Заберите награду";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit8), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit8_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("Quick Move Item", null) + " содержимое стойки, чтобы переместить каждый предмет из стойки в ваш инвентарь.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit8), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit8_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отлично!";
            return false;
        }
    }

    // Patch For HallwayConduit9
    [HarmonyPatch(typeof(HallwayConduit9), "get_ObjectiveName")]
    public static class Patch_HallwayConduit9_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Продолжайте путь к вашему кораблю";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit9), "get_ObjectiveDesc")]
    public static class Patch_HallwayConduit9_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Это было поучительное отвлечение. Вы многое узнали. Продолжайте путь к вашему кораблю у причала.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HallwayConduit9), "get_ObjectiveDescComplete")]
    public static class Patch_HallwayConduit9_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "";
            return false;
        }
    }



    // Patch For HelmetAtmosphereUnsafe
    [HarmonyPatch(typeof(HelmetAtmosphereUnsafe), "get_ObjectiveName")]
    public static class Patch_HelmetAtmosphereUnsafe_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Атмосфера шлема небезопасна";
            return false;
        }
    }

    [HarmonyPatch(typeof(HelmetAtmosphereUnsafe), "get_ObjectiveDesc")]
    public static class Patch_HelmetAtmosphereUnsafe_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Найдите комнату с безопасным уровнем O2 (20 кПа+) и CO2 (<1 кПа) и снимите шлем.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HelmetAtmosphereUnsafe), "get_ObjectiveDescComplete")]
    public static class Patch_HelmetAtmosphereUnsafe_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Шлем снят.";
            return false;
        }
    }


    // Patch For HighlightObjects
    [HarmonyPatch(typeof(HighlightObjects), "get_ObjectiveName")]
    public static class Patch_HighlightObjects_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подсветка интерактивных объектов и горячих клавиш";
            return false;
        }
    }

    [HarmonyPatch(typeof(HighlightObjects), "get_ObjectiveDesc")]
    public static class Patch_HighlightObjects_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Удерживайте " + InputManager.GetGlyphString("Show Hotkeys & Interactables", null) + ", чтобы увидеть горячие клавиши и ближайшие интерактивные объекты.";
            return false;
        }
    }

    [HarmonyPatch(typeof(HighlightObjects), "get_ObjectiveDescComplete")]
    public static class Patch_HighlightObjects_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Интерактивные объекты и горячие клавиши подсвечены.";
            return false;
        }
    }


    // Patch For LeftThrust
    [HarmonyPatch(typeof(LeftThrust), "get_ObjectiveName")]
    public static class Patch_LeftThrust_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибровать левую тягу";
            return false;
        }
    }

    [HarmonyPatch(typeof(LeftThrust), "get_ObjectiveDesc")]
    public static class Patch_LeftThrust_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите клавишу \"" + InputManager.GetGlyphString("Thrust Left", null) + "\", чтобы снова снизить VREL ниже 100 м/с (с целевым OKLG).";
            return false;
        }
    }

    [HarmonyPatch(typeof(LeftThrust), "get_ObjectiveDescComplete")]
    public static class Patch_LeftThrust_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Левая тяга откалибрована.";
            return false;
        }
    }

    // Patch For LootTheBridge
    [HarmonyPatch(typeof(LootTheBridge), "get_ObjectiveName")]
    public static class Patch_LootTheBridge_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Разграбить мостик";
            return false;
        }
    }

    [HarmonyPatch(typeof(LootTheBridge), "get_ObjectiveDesc")]
    public static class Patch_LootTheBridge_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Кому бы это ни принадлежало, им это больше не нужно. Возьмите всё, что сможете, из шкафчиков и с мостика.";
            return false;
        }
    }

    [HarmonyPatch(typeof(LootTheBridge), "get_ObjectiveDescComplete")]
    public static class Patch_LootTheBridge_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Когда будете готовы, возвращайтесь на KLEG.";
            return false;
        }
    }

    // Patch For MatchSpeed
    [HarmonyPatch(typeof(MatchSpeed), "get_ObjectiveName")]
    public static class Patch_MatchSpeed_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сравнять скорость";
            return false;
        }
    }

    [HarmonyPatch(typeof(MatchSpeed), "get_ObjectiveDesc")]
    public static class Patch_MatchSpeed_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Включите \"Сравнять скорость\" (" + InputManager.GetGlyphString("Toggle station keeping", null) + "), чтобы автоматически сравнивать скорость и вращение с вашей целью.";
            return false;
        }
    }

    [HarmonyPatch(typeof(MatchSpeed), "get_ObjectiveDescComplete")]
    public static class Patch_MatchSpeed_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сравнение скорости включено.";
            return false;
        }
    }

    // Patch For MouseoverObjective
    [HarmonyPatch(typeof(MouseoverObjective), "get_ObjectiveName")]
    public static class Patch_MouseoverObjective_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Найти цель задания";
            return false;
        }
    }

    [HarmonyPatch(typeof(MouseoverObjective), "get_ObjectiveDesc")]
    public static class Patch_MouseoverObjective_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите " + InputManager.GetGlyphString("Click", null) + " на этом окне, чтобы сфокусироваться на цели задания.";
            return false;
        }
    }

    [HarmonyPatch(typeof(MouseoverObjective), "get_ObjectiveDescComplete")]
    public static class Patch_MouseoverObjective_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Цель задания выбрана.";
            return false;
        }
    }

    // Patch For NavUseShow
    [HarmonyPatch(typeof(NavUseShow), "get_ObjectiveName")]
    public static class Patch_NavUseShow_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Использовать навигационную станцию";
            return false;
        }
    }

    [HarmonyPatch(typeof(NavUseShow), "get_ObjectiveDesc")]
    public static class Patch_NavUseShow_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("RightClick", null) + " навигационную станцию, затем выберите Использовать.";
            return false;
        }
    }

    [HarmonyPatch(typeof(NavUseShow), "get_ObjectiveDescComplete")]
    public static class Patch_NavUseShow_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Готов.";
            return false;
        }
    }

    // Patch For NavWalk
    [HarmonyPatch(typeof(NavWalk), "get_ObjectiveName")]
    public static class Patch_NavWalk_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Посетите ваш корабль";
            return false;
        }
    }

    [HarmonyPatch(typeof(NavWalk), "get_ObjectiveDesc")]
    public static class Patch_NavWalk_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Пройдите через станцию OKLG. Поднимитесь на борт вашего корабля, пристыкованного к шлюзу.";
            return false;
        }
    }

    [HarmonyPatch(typeof(NavWalk), "get_ObjectiveDescComplete")]
    public static class Patch_NavWalk_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Корабль найден.";
            return false;
        }
    }

    // Patch For OpenInventory
    [HarmonyPatch(typeof(OpenInventory), "get_ObjectiveName")]
    public static class Patch_OpenInventory_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откройте ваш инвентарь";
            return false;
        }
    }

    [HarmonyPatch(typeof(OpenInventory), "get_ObjectiveDesc")]
    public static class Patch_OpenInventory_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите " + InputManager.GetGlyphString("Click", null) + " на ваш портрет или нажмите '" + InputManager.GetGlyphString("Player Inventory", null) + "', чтобы посмотреть предметы, которые у вас есть.";
            return false;
        }
    }

    [HarmonyPatch(typeof(OpenInventory), "get_ObjectiveDescComplete")]
    public static class Patch_OpenInventory_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Инвентарь просмотрен.";
            return false;
        }
    }

    // Patch For PickUpPermit
    [HarmonyPatch(typeof(PickUpPermit), "get_ObjectiveName")]
    public static class Patch_PickUpPermit_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подберите разрешение";
            return false;
        }
    }

    [HarmonyPatch(typeof(PickUpPermit), "get_ObjectiveDesc")]
    public static class Patch_PickUpPermit_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подберите жёлтое разрешение на утилизацию на полу. " + InputManager.GetGlyphString("RightClick", null) + " его и выберите \"Прочитать\".";
            return false;
        }
    }

    [HarmonyPatch(typeof(PickUpPermit), "get_ObjectiveDescComplete")]
    public static class Patch_PickUpPermit_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Разрешение получено.";
            return false;
        }
    }


    // Patch For PickUpToolbox
    [HarmonyPatch(typeof(PickUpToolbox), "get_ObjectiveName")]
    public static class Patch_PickUpToolbox_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подберите ваш ящик с инструментами";
            return false;
        }
    }

    [HarmonyPatch(typeof(PickUpToolbox), "get_ObjectiveDesc")]
    public static class Patch_PickUpToolbox_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Поместите жёлтый ящик с инструментами в слот для рук.";
            return false;
        }
    }

    [HarmonyPatch(typeof(PickUpToolbox), "get_ObjectiveDescComplete")]
    public static class Patch_PickUpToolbox_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Инструменты получены.";
            return false;
        }
    }


    // Patch For PrepareToExploreDerelict
    [HarmonyPatch(typeof(PrepareToExploreDerelict), "get_ObjectiveName")]
    public static class Patch_PrepareToExploreDerelict_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Надеть скафандр";
            return false;
        }
    }

    [HarmonyPatch(typeof(PrepareToExploreDerelict), "get_ObjectiveDesc")]
    public static class Patch_PrepareToExploreDerelict_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Покиньте навигационную станцию, откройте инвентарь, снимите комбинезон и обувь, наденьте скафандр и шлем.";
            return false;
        }
    }

    [HarmonyPatch(typeof(PrepareToExploreDerelict), "get_ObjectiveDescComplete")]
    public static class Patch_PrepareToExploreDerelict_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Готов к выходу в открытый космос.";
            return false;
        }
    }


    // Patch For ReachBridge2
    [HarmonyPatch(typeof(ReachBridge2), "get_ObjectiveName")]
    public static class Patch_ReachBridge2_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Доступ к мостику 2";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridge2), "get_ObjectiveDesc")]
    public static class Patch_ReachBridge2_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Демонтируйте кабель-канал через панель быстрых действий.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridge2), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridge2_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Кабель-канал демонтирован.";
            return false;
        }
    }


    // Patch For ReachBridgeAlternate1
    [HarmonyPatch(typeof(ReachBridgeAlternate1), "get_ObjectiveName")]
    public static class Patch_ReachBridgeAlternate1_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Найти другой путь";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeAlternate1), "get_ObjectiveDesc")]
    public static class Patch_ReachBridgeAlternate1_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Как вариант, если поискать вокруг, возможно, есть другой путь мимо двери...";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeAlternate1), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridgeAlternate1_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вот это уже кое-что.";
            return false;
        }
    }


    // Patch For ReachBridgeAlternate2
    [HarmonyPatch(typeof(ReachBridgeAlternate2), "get_ObjectiveName")]
    public static class Patch_ReachBridgeAlternate2_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Доступ к мостику 5";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeAlternate2), "get_ObjectiveDesc")]
    public static class Patch_ReachBridgeAlternate2_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Используйте лом, чтобы взломать дверь на мостик.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeAlternate2), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridgeAlternate2_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Мостик достигнут.";
            return false;
        }
    }

    // Patch For ReachBridgeDoorStep
    [HarmonyPatch(typeof(ReachBridgeDoorStep), "get_ObjectiveName")]
    public static class Patch_ReachBridgeDoorStep_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Доступ к мостику 4";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeDoorStep), "get_ObjectiveDesc")]
    public static class Patch_ReachBridgeDoorStep_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Пройдите через дверь.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeDoorStep), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridgeDoorStep_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Мостик достигнут.";
            return false;
        }
    }

    // Patch For ReachBridgeInstallStep
    [HarmonyPatch(typeof(ReachBridgeInstallStep), "get_ObjectiveName")]
    public static class Patch_ReachBridgeInstallStep_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Доступ к мостику 3";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeInstallStep), "get_ObjectiveDesc")]
    public static class Patch_ReachBridgeInstallStep_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откройте визор и определите недостающую цепь. Установите свободный кабель-канал, чтобы подать питание на дверь мостика.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeInstallStep), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridgeInstallStep_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Дверь мостика запитана.";
            return false;
        }
    }

    // Patch For ReachBridgeTest
    [HarmonyPatch(typeof(ReachBridgeTest), "get_ObjectiveName")]
    public static class Patch_ReachBridgeTest_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Доступ к мостику";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeTest), "get_ObjectiveDesc")]
    public static class Patch_ReachBridgeTest_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подайте питание на дверь в конце длинного коридора. При необходимости \"одолжите\" лишний кабель-канал.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReachBridgeTest), "get_ObjectiveDescComplete")]
    public static class Patch_ReachBridgeTest_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вот этот.";
            return false;
        }
    }

    // Patch For RearThrust
    [HarmonyPatch(typeof(RearThrust), "get_ObjectiveName")]
    public static class Patch_RearThrust_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибровать заднюю тягу";
            return false;
        }
    }

    [HarmonyPatch(typeof(RearThrust), "get_ObjectiveDesc")]
    public static class Patch_RearThrust_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите клавишу \"" + InputManager.GetGlyphString("Thrust Down", null) + "\", чтобы двигаться назад до скорости 200 м/с VREL (с целью OKLG).";
            return false;
        }
    }

    [HarmonyPatch(typeof(RearThrust), "get_ObjectiveDescComplete")]
    public static class Patch_RearThrust_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Задняя тяга откалибрована.";
            return false;
        }
    }

    // Patch For RefuelAtKiosk
    [HarmonyPatch(typeof(RefuelAtKiosk), "get_ObjectiveName")]
    public static class Patch_RefuelAtKiosk_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Заправиться на топливном киоске";
            return false;
        }
    }

    [HarmonyPatch(typeof(RefuelAtKiosk), "get_ObjectiveDesc")]
    public static class Patch_RefuelAtKiosk_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Посетите оранжевый киоск \"Услуги по стыковке и заправке\" и заправьте свой корабль.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RefuelAtKiosk), "get_ObjectiveDescComplete")]
    public static class Patch_RefuelAtKiosk_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Корабль заправлен.";
            return false;
        }
    }

    // Patch For RequestClearance
    [HarmonyPatch(typeof(RequestClearance), "get_ObjectiveName")]
    public static class Patch_RequestClearance_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Запросить разрешение на отстыковку";
            return false;
        }
    }

    [HarmonyPatch(typeof(RequestClearance), "get_ObjectiveDesc")]
    public static class Patch_RequestClearance_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Выберите \"Запросить разрешение на отстыковку\". Затем, чтобы отстыковаться, нажмите кнопку CLAMPS.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RequestClearance), "get_ObjectiveDescComplete")]
    public static class Patch_RequestClearance_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отстыковано от OKLG.";
            return false;
        }
    }

    // Patch For RestoreNavStation
    [HarmonyPatch(typeof(RestoreNavStation), "get_ObjectiveName")]
    public static class Patch_RestoreNavStation_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Восстановить навигационную станцию";
            return false;
        }
    }

    [HarmonyPatch(typeof(RestoreNavStation), "get_ObjectiveDesc")]
    public static class Patch_RestoreNavStation_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("RightClick", null) + " навигационную станцию, чтобы ВОССТАНОВИТЬ её, убрав немного износа и повреждений.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RestoreNavStation), "get_ObjectiveDescComplete")]
    public static class Patch_RestoreNavStation_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Восстановление важно, но требует времени.";
            return false;
        }
    }

    // Patch For RestorePower
    [HarmonyPatch(typeof(RestorePower), "get_ObjectiveName")]
    public static class Patch_RestorePower_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Установите кабель-канал для подачи питания на дверь.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RestorePower), "get_ObjectiveDesc")]
    public static class Patch_RestorePower_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Подайте питание на дверь, установив свободный кабель-канал в ближайший разрыв.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RestorePower), "get_ObjectiveDescComplete")]
    public static class Patch_RestorePower_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Одна дверь готова.";
            return false;
        }
    }



    // Patch For ReturnToKLEG
    [HarmonyPatch(typeof(ReturnToKLEG), "get_ObjectiveName")]
    public static class Patch_ReturnToKLEG_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вернуться на K-LEG";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReturnToKLEG), "get_ObjectiveDesc")]
    public static class Patch_ReturnToKLEG_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Летите обратно и состыкуйтесь со станцией OKLG, чтобы продать свои трофеи.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ReturnToKLEG), "get_ObjectiveDescComplete")]
    public static class Patch_ReturnToKLEG_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вернулись на OKLG для продажи трофеев.";
            return false;
        }
    }

    // Patch For RightThrust
    [HarmonyPatch(typeof(RightThrust), "get_ObjectiveName")]
    public static class Patch_RightThrust_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откалибровать правую тягу";
            return false;
        }
    }

    [HarmonyPatch(typeof(RightThrust), "get_ObjectiveDesc")]
    public static class Patch_RightThrust_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите клавишу \"" + InputManager.GetGlyphString("Thrust Right", null) + "\", чтобы двигаться вправо до скорости 200 м/с VREL (с целью OKLG).";
            return false;
        }
    }

    [HarmonyPatch(typeof(RightThrust), "get_ObjectiveDescComplete")]
    public static class Patch_RightThrust_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Правая тяга откалибрована.";
            return false;
        }
    }


    // Patch For RosterPermission
    [HarmonyPatch(typeof(RosterPermission), "get_ObjectiveName")]
    public static class Patch_RosterPermission_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Изменить разрешения экипажа";
            return false;
        }
    }

    [HarmonyPatch(typeof(RosterPermission), "get_ObjectiveDesc")]
    public static class Patch_RosterPermission_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Откройте интерфейс <color=#FFCC00>РОСТЕР</color> и измените разрешения на доступ к шлюзу.";
            return false;
        }
    }

    [HarmonyPatch(typeof(RosterPermission), "get_ObjectiveDescComplete")]
    public static class Patch_RosterPermission_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Разрешения изменены.";
            return false;
        }
    }

    // Patch For SelectCompartment
    [HarmonyPatch(typeof(SelectCompartment), "get_ObjectiveName")]
    public static class Patch_SelectCompartment_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Проверить атмосферу в комнате";
            return false;
        }
    }

    [HarmonyPatch(typeof(SelectCompartment), "get_ObjectiveDesc")]
    public static class Patch_SelectCompartment_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("RightClick", null) + " несколько раз по тому же месту в комнате, пока МПП не отобразит 'Отсек'.";
            return false;
        }
    }

    [HarmonyPatch(typeof(SelectCompartment), "get_ObjectiveDescComplete")]
    public static class Patch_SelectCompartment_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отсек выбран.";
            return false;
        }
    }

    // Patch For SelectMTT
    [HarmonyPatch(typeof(SelectMTT), "get_ObjectiveName")]
    public static class Patch_SelectMTT_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Открыть мега-подсказку.";
            return false;
        }
    }

    [HarmonyPatch(typeof(SelectMTT), "get_ObjectiveDesc")]
    public static class Patch_SelectMTT_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("RightClick", null) + " на любой объект, чтобы открыть мега-подсказку (МПП).";
            return false;
        }
    }

    [HarmonyPatch(typeof(SelectMTT), "get_ObjectiveDescComplete")]
    public static class Patch_SelectMTT_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "МПП открыта.";
            return false;
        }
    }

    // Patch For SellSalvageAtKiosk
    [HarmonyPatch(typeof(SellSalvageAtKiosk), "get_ObjectiveName")]
    public static class Patch_SellSalvageAtKiosk_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Продать утиль в киоске";
            return false;
        }
    }

    [HarmonyPatch(typeof(SellSalvageAtKiosk), "get_ObjectiveDesc")]
    public static class Patch_SellSalvageAtKiosk_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Посетите жёлтый лицензированный киоск или серый киоск для металлолома и продайте любой предмет.";
            return false;
        }
    }

    [HarmonyPatch(typeof(SellSalvageAtKiosk), "get_ObjectiveDescComplete")]
    public static class Patch_SellSalvageAtKiosk_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Продажа утиля завершена.";
            return false;
        }
    }

    // Patch For StopSpin
    [HarmonyPatch(typeof(StopSpin), "get_ObjectiveName")]
    public static class Patch_StopSpin_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Остановить вращение";
            return false;
        }
    }

    [HarmonyPatch(typeof(StopSpin), "get_ObjectiveDesc")]
    public static class Patch_StopSpin_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Удерживайте \"" + InputManager.GetGlyphString("Attitude", null) + "\", пока корабль не перестанет вращаться.";
            return false;
        }
    }

    [HarmonyPatch(typeof(StopSpin), "get_ObjectiveDescComplete")]
    public static class Patch_StopSpin_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вращение остановлено.";
            return false;
        }
    }


    // Patch For SwitchNav
    [HarmonyPatch(typeof(SwitchNav), "get_ObjectiveName")]
    public static class Patch_SwitchNav_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключиться на навигационный экран";
            return false;
        }
    }

    [HarmonyPatch(typeof(SwitchNav), "get_ObjectiveDesc")]
    public static class Patch_SwitchNav_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите \"Переключиться на навигационное управление\" в левой части консоли, чтобы вернуться к навигационному управлению.";
            return false;
        }
    }

    [HarmonyPatch(typeof(SwitchNav), "get_ObjectiveDescComplete")]
    public static class Patch_SwitchNav_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключено на навигационный экран.";
            return false;
        }
    }


    // Patch For SwitchToComms
    [HarmonyPatch(typeof(SwitchToComms), "get_ObjectiveName")]
    public static class Patch_SwitchToComms_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключиться на управление связью";
            return false;
        }
    }

    [HarmonyPatch(typeof(SwitchToComms), "get_ObjectiveDesc")]
    public static class Patch_SwitchToComms_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = InputManager.GetGlyphString("Click", null) + " на кнопку \"Управление связью\" в правой части навигационной консоли, чтобы увидеть управление связью.";
            return false;
        }
    }

    [HarmonyPatch(typeof(SwitchToComms), "get_ObjectiveDescComplete")]
    public static class Patch_SwitchToComms_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключено на экран связи.";
            return false;
        }
    }

    // Patch For TargetDerelict
    [HarmonyPatch(typeof(TargetDerelict), "get_ObjectiveName")]
    public static class Patch_TargetDerelict_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нацелиться на ближайший заброшенный корабль";
            return false;
        }
    }

    [HarmonyPatch(typeof(TargetDerelict), "get_ObjectiveDesc")]
    public static class Patch_TargetDerelict_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отдалите вид. Найдите навигационный контакт с пометкой TUTORIAL DERELICT на кладбище кораблей. Выберите его.";
            return false;
        }
    }

    [HarmonyPatch(typeof(TargetDerelict), "get_ObjectiveDescComplete")]
    public static class Patch_TargetDerelict_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "TUTORIAL DERELICT выбран.";
            return false;
        }
    }

    // Patch For TargetOKLG
    [HarmonyPatch(typeof(TargetOKLG), "get_ObjectiveName")]
    public static class Patch_TargetOKLG_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нацелиться на станцию K-Leg";
            return false;
        }
    }

    [HarmonyPatch(typeof(TargetOKLG), "get_ObjectiveDesc")]
    public static class Patch_TargetOKLG_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите на белый ромб с надписью OKLG, чтобы нацелиться на него и получить подробную информацию.";
            return false;
        }
    }

    [HarmonyPatch(typeof(TargetOKLG), "get_ObjectiveDescComplete")]
    public static class Patch_TargetOKLG_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Станция K-Leg выбрана.";
            return false;
        }
    }

    // Patch For ToggleLightSwitch
    [HarmonyPatch(typeof(ToggleLightSwitch), "get_ObjectiveName")]
    public static class Patch_ToggleLightSwitch_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переключить выключатель";
            return false;
        }
    }

    [HarmonyPatch(typeof(ToggleLightSwitch), "get_ObjectiveDesc")]
    public static class Patch_ToggleLightSwitch_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите " + InputManager.GetGlyphString("RightClick", null) + " на ближайшем выключателе питания. Выберите Переключить питание, чтобы включить свет.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ToggleLightSwitch), "get_ObjectiveDescComplete")]
    public static class Patch_ToggleLightSwitch_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Свет включён.";
            return false;
        }
    }

    // Patch For ToggleOffMatchSpeed
    [HarmonyPatch(typeof(ToggleOffMatchSpeed), "get_ObjectiveName")]
    public static class Patch_ToggleOffMatchSpeed_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Выключить сравнение скорости";
            return false;
        }
    }

    [HarmonyPatch(typeof(ToggleOffMatchSpeed), "get_ObjectiveDesc")]
    public static class Patch_ToggleOffMatchSpeed_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Не забудьте выключить сравнение скорости, прежде чем снова двигать корабль.";
            return false;
        }
    }

    [HarmonyPatch(typeof(ToggleOffMatchSpeed), "get_ObjectiveDescComplete")]
    public static class Patch_ToggleOffMatchSpeed_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сравнение скорости выключено.";
            return false;
        }
    }

    // Patch For TravelToDerelict
    [HarmonyPatch(typeof(TravelToDerelict), "get_ObjectiveName")]
    public static class Patch_TravelToDerelict_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переместиться в диапазон стыковки";
            return false;
        }
    }

    [HarmonyPatch(typeof(TravelToDerelict), "get_ObjectiveDesc")]
    public static class Patch_TravelToDerelict_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переместитесь в пределах RNG < 5 км от вашей цели и снизьте скорость, пока VREL не станет ниже 100 м/с (с выбранным пунктом назначения).";
            return false;
        }
    }

    [HarmonyPatch(typeof(TravelToDerelict), "get_ObjectiveDescComplete")]
    public static class Patch_TravelToDerelict_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Переместились в диапазон стыковки.";
            return false;
        }
    }

    // Patch For TutorialEnd
    [HarmonyPatch(typeof(TutorialEnd), "get_ObjectiveName")]
    public static class Patch_TutorialEnd_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Обучение завершено";
            return false;
        }
    }

    [HarmonyPatch(typeof(TutorialEnd), "get_ObjectiveDesc")]
    public static class Patch_TutorialEnd_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Вы достигли конца обучения. Отсюда ваша цель — звёзды.";
            return false;
        }
    }

    [HarmonyPatch(typeof(TutorialEnd), "get_ObjectiveDescComplete")]
    public static class Patch_TutorialEnd_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Удачи.";
            return false;
        }
    }

    // Patch For UnpaidDockingFees
    [HarmonyPatch(typeof(UnpaidDockingFees), "get_ObjectiveName")]
    public static class Patch_UnpaidDockingFees_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Неоплаченные сборы за стыковку";
            return false;
        }
    }

    [HarmonyPatch(typeof(UnpaidDockingFees), "get_ObjectiveDesc")]
    public static class Patch_UnpaidDockingFees_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Посетите топливный киоск, чтобы оплатить сборы за стыковку";
            return false;
        }
    }

    [HarmonyPatch(typeof(UnpaidDockingFees), "get_ObjectiveDescComplete")]
    public static class Patch_UnpaidDockingFees_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сборы за стыковку оплачены!";
            return false;
        }
    }

    // Patch For UnpauseWorld
    [HarmonyPatch(typeof(UnpauseWorld), "get_ObjectiveName")]
    public static class Patch_UnpauseWorld_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Возобновить игру";
            return false;
        }
    }

    [HarmonyPatch(typeof(UnpauseWorld), "get_ObjectiveDesc")]
    public static class Patch_UnpauseWorld_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите '" + InputManager.GetGlyphString("Pause", null) + "' или треугольную кнопку \"play\" на панели времени внизу справа.";
            return false;
        }
    }

    [HarmonyPatch(typeof(UnpauseWorld), "get_ObjectiveDescComplete")]
    public static class Patch_UnpauseWorld_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Игра возобновлена.";
            return false;
        }
    }

    // Patch For VisualisePower
    [HarmonyPatch(typeof(VisualisePower), "get_ObjectiveName")]
    public static class Patch_VisualisePower_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Визуализировать сети питания";
            return false;
        }
    }

    [HarmonyPatch(typeof(VisualisePower), "get_ObjectiveDesc")]
    public static class Patch_VisualisePower_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Нажмите " + InputManager.GetGlyphString("Toggle PDA Power Vizor", null) + ", чтобы открыть управление визором. Выберите визуализацию питания.";
            return false;
        }
    }

    [HarmonyPatch(typeof(VisualisePower), "get_ObjectiveDescComplete")]
    public static class Patch_VisualisePower_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Сети питания визуализированы.";
            return false;
        }
    }

    // Patch For Zoom
    [HarmonyPatch(typeof(Zoom), "get_ObjectiveName")]
    public static class Patch_Zoom_ObjectiveName
    {
        static bool Prefix(ref string __result)
        {
            __result = "Отрегулируйте масштаб навигационной станции";
            return false;
        }
    }

    [HarmonyPatch(typeof(Zoom), "get_ObjectiveDesc")]
    public static class Patch_Zoom_ObjectiveDesc
    {
        static bool Prefix(ref string __result)
        {
            __result = "Приближайте или отдаляйте вид навигационной станции с помощью колёсика мыши. Удерживайте клавишу Shift для более быстрого масштабирования.\n\nПриближайтесь, пока расстояние не станет менее 6 км.";
            return false;
        }
    }

    [HarmonyPatch(typeof(Zoom), "get_ObjectiveDescComplete")]
    public static class Patch_Zoom_ObjectiveDescComplete
    {
        static bool Prefix(ref string __result)
        {
            __result = "Всё становится чётче.";
            return false;
        }
    }



    // -
    // GrammarUtils

    // Patch for CondOwner.LogMessage – замена "are no longer" на русский
    [HarmonyPatch(typeof(CondOwner), nameof(CondOwner.LogMessage))]
    public static class Patch_CondOwner_LogMessage
    {
        static void Prefix(ref string strMsg)
        {
            if (!string.IsNullOrEmpty(strMsg))
            {
                strMsg = strMsg.Replace(" are no longer ", " больше не ");
                strMsg = strMsg.Replace(" are ", " ");
            }
        }
    }
}