using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private static PlayerData _instance;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateNewPlayerData();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Contains all the health segments that the player current has.
    /// </summary>
    [SerializeField]
    private List<HealthSegment> _healthSegments = new List<HealthSegment>();
    public HealthSegment[] HealthSegments => _healthSegments.ToArray();
    /// <summary>
    /// The current total number of health segments that the player has.
    /// </summary>
    public int NumMaxHealthSegments => _healthSegments.Count();
    /// <summary>
    /// The number of health segments that are draining.
    /// </summary>
    public int DrainingHealthSegments => _healthSegments.Count(segment => segment.Draining);
    /// <summary>
    /// The number of segments that are full/not draining.
    /// </summary>
    public int FullHealthSegments => NumMaxHealthSegments - DrainingHealthSegments;
    /// <summary>
    /// Whether all health segments are draining.
    /// </summary>
    public bool AllHealthSegmentsDraining => DrainingHealthSegments == NumMaxHealthSegments;
    /// <summary>
    /// Whether the player is at full health with no segments draining.
    /// </summary>
    public bool FullHealth { get => DrainingHealthSegments <= 0; }

    public static PlayerData CreateNewPlayerData()
    {
        _instance = new PlayerData();
        for (int i = 0; i < 5; i++)
            _instance._healthSegments.Add(new HealthSegment());
        return _instance;
    }

    /// <summary>
    /// Start draining health.
    /// </summary>
    /// <param name="damageAmount">The number of health segments to start draining.</param>
    public void StartDrainingHealth(int damageAmount)
    {
        if (AllHealthSegmentsDraining) return;

        var lastFullSegment = _healthSegments.Last(segment => segment.Full);
        var indexOfLastSegment = _healthSegments.IndexOf(lastFullSegment);
        _healthSegments.GetRange(indexOfLastSegment - damageAmount + 1, damageAmount).ForEach(segment => segment.StartDraining());
    }

    /// <summary>
    /// Instantly and completely drain an amount of health.
    /// </summary>
    /// <param name="damageAmount">The number of health segments to instantly drain.</param>
    public void InstantlyDrainHealth(int damageAmount)
    {
        if (AllHealthSegmentsDraining) return;

        var lastFullSegment = _healthSegments.Last(segment => segment.Full);
        var indexOfLastSegment = _healthSegments.IndexOf(lastFullSegment);
        _healthSegments.GetRange(indexOfLastSegment - damageAmount + 1, damageAmount).ForEach(segment => segment.InstantlyDrain());
    }

    /// <summary>
    /// Start restoring health.
    /// </summary>
    /// <param name="restoreAmount">The number of health segments to start restoring.</param>
    public void StartRestoringHealth(int restoreAmount)
    {
        if (FullHealth) return;

        var lastDrainingOrEmptySegment = _healthSegments.Last(segment => segment.Draining || segment.Empty);
        var indexOfDrainingOrEmptySegment = _healthSegments.IndexOf(lastDrainingOrEmptySegment);
        _healthSegments.GetRange(indexOfDrainingOrEmptySegment, restoreAmount).ForEach(segment => segment.StartRestoring());
    }

    /// <summary>
    /// Instantly and completely restore an amount of health.
    /// </summary>
    /// <param name="restoreAmount">The number of health segments to instantly restore.</param>
    public void InstantlyRestoreHealth(int restoreAmount)
    {
        if (FullHealth) return;

        var lastDrainingOrEmptySegment = _healthSegments.Last(segment => segment.Draining || segment.Empty);
        var indexOfDrainingOrEmptySegment = _healthSegments.IndexOf(lastDrainingOrEmptySegment);
        _healthSegments.GetRange(indexOfDrainingOrEmptySegment, restoreAmount).ForEach(segment => segment.InstantlyRestore());
    }

    /// <summary>
    /// Add new health segment(s) to the player's total health.
    /// </summary>
    /// <param name="numSegments">The number of segments to add.</param>
    public void AddHealthSegment(int numSegments)
    {
        _healthSegments.ForEach(segment => segment.InstantlyRestore());
        for (int i = 0; i < numSegments; i++)
        {
            var referenceSegment = _healthSegments[0];
            var newSegment = new HealthSegment();
            newSegment.maxSegmentSize = referenceSegment.maxSegmentSize;
            newSegment.DrainRate = referenceSegment.DrainRate;
            newSegment.RestoreRate = referenceSegment.RestoreRate;
            newSegment.segmentIndex = _healthSegments.Count();
            _healthSegments.Add(newSegment);
        }
    }

    /// <summary>
    /// Remove health segment(s) from the player's total health.
    /// </summary>
    /// <param name="numSegments">The number of segments to remove.</param>
    public void RemoveHealthSegment(int numSegments)
    {
        _healthSegments.RemoveRange(_healthSegments.Count() - numSegments, numSegments);
    }
}
