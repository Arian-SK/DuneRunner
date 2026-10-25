using UnityEngine;

// Ground obstacle: you must jump over it
public class Cactus : Interactable
{
    public int damage = 1;

    protected override void Interact(PlayerController player)
    {
        player.TakeDamage(damage);
    }
}
