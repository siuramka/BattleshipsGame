﻿namespace backend.Models.Entity;

public class Player
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool HasTurn { get; set; } = false;
    public GameBoard OwnBoard { get; } = new();
    public bool HasSetupShips { get; set; } = false;
    public bool IsInTestMode { get; set; } = false;
}