using UnityEngine;

public class AnimationController
{
    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    private Animator animator;

    private readonly string SPEED = "Speed";

    public void PlayWalk(Vector3 direction, float speed)
    {
        animator.SetFloat(SPEED, speed);
    }
}
