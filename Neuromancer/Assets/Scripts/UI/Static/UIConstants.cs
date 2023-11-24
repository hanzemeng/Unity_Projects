using UnityEngine;
using static Neuromancer.Unit;

public class UIConstants {
    public static readonly Color neutralColor = new Color(0.8f, 0.8f, 0.8f);
    public static readonly Color heroColor = new Color(0f, 0.2f, 0.8f);
    public static readonly Color allyColor = new Color(0.2f, 0.8f, 0f);
    public static readonly Color enemyColor = new Color(0.8f, 0.2f, 0f);

    public static readonly Color defaultReticleColor = heroColor;
    public static readonly Color commandColor = allyColor;
    public static readonly Color targetColor = enemyColor;
    public static readonly Color castingColor = new Color(0.6f, 0f, 0.6f);

    public static readonly float outlineWidth = 1.5f;
    public static readonly float shadingFactor = 0.5f;

    // Follows the order of the BuffType enum defined in Buff.cs
    // Speed, Heal, Attack, Defense, Pull, Push
    public static readonly Color[] buffColors = {new(127, 0, 255, 0.5f), new(255, 0, 0, 0.5f), new(0, 255, 0, 0.5f),
                                                 new(0, 0, 255, 0.5f), new(0, 255, 255, 0.5f), new(255, 255, 0, 0.5f)};

    public static Color GetShadedColor(Color c) {
        return Color.white - shadingFactor*Color.white + shadingFactor*c;
    }

    public static Color GetColorFromUnitType(Transform t) {
        if (IsAlly(t)) {
            return allyColor;
        } else if (IsEnemy(t)) {
            return enemyColor;
        } else if (IsHero(t)) {
            return heroColor;
        } else {
            return neutralColor;
        }
    }

    public static Color GetOppositeColorFromUnitType(Transform t) {
        if (IsFriend(t)) {
            return enemyColor;
        } else if (IsEnemy(t)) {
            return new Color(1f, 0.75f, 0f);
        } else {
            return neutralColor;
        }
    }
}