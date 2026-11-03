using System;
using UnityEngine;

/// <summary>\
/// Used to execute a method when entering a dialogue sequence.
/// For example, disabling player movement when entering a dialogue requires subscribing to a method.
/// </summary>
public static class SpeakerInputLock
{
    public static bool IsLocked { get; private set; }
    public static event Action<bool> OnLockChanged;

    public static void Lock()
    {
        if (IsLocked) return;
        IsLocked = true;
        OnLockChanged?.Invoke(true);
    }

    public static void Unlock()
    {
        if (!IsLocked) return;
        IsLocked = false;
        OnLockChanged?.Invoke(false);
    }
}