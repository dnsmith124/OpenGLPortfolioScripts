using UnityEngine;

public static class AnimationUtils
{
    public static float GetAnimationClipLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clipName)
            {
                return clips[i].length;
            }
        }
        Debug.LogError("Could not find clip: " + clipName);
        return 0;
    }
}
