using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AutoCreateAnimationEditor : Editor
{
    //生成出的AnimationController的路径
    private static string _animationControllerPath = "Assets/Animation";

    //生成出的Animation的路径
    private static string _animationPath = "Assets/Animation";

    //美术给的原始图片路径
    private static string _spriteRootPath = "Assets/Texture";

    [MenuItem("Tools/CreateAnimatorController")]
    static void CreateAnimation()
    {
        DirectoryInfo root = new DirectoryInfo(_spriteRootPath);

        foreach (DirectoryInfo roleFolder in root.GetDirectories())
        {
            List<AnimationClip> clipsList = new List<AnimationClip>();

            foreach (DirectoryInfo actionFolder in roleFolder.GetDirectories())
            {
                foreach (DirectoryInfo unitActionFolder in actionFolder.GetDirectories())
                {
                    clipsList.Add(CreateAnimationClip(unitActionFolder));
                }
            }

            CreateAnimationController(clipsList, roleFolder.Name);
        }
    }

    static AnimationClip CreateAnimationClip(DirectoryInfo unitActionFolder)
    {
        string animationName = unitActionFolder.Name.ToLower();

        FileInfo[] sprites = unitActionFolder.GetFiles("*.png");
        Array.Sort(sprites, new FileComparer());

        AnimationClip clip = new AnimationClip();
        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        string roleFolderName = unitActionFolder.Parent.Parent.Name;
        string actionFolderName = unitActionFolder.Parent.Name;

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];

        // 动画帧率 表示一秒播放几张图片
        int frame = 10;
        float frameTime = 1f / frame;

        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"{_spriteRootPath}/{roleFolderName}/{actionFolderName}/{unitActionFolder.Name}/{sprites[i].Name}");

            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = frameTime * i,
                value = sprite
            };
        }

        clip.frameRate = frame;

        // 设置循环播放
        AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(clip);
        clipSetting.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, clipSetting);

        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

        // 创建文件夹
        string animationClipPath = $"{_animationPath}/{roleFolderName}/{actionFolderName}";
        Directory.CreateDirectory($"{animationClipPath}");

        // 创建AnimationClip文件
        string file = $"{animationClipPath}/{roleFolderName}_{animationName}.anim";
        AssetDatabase.CreateAsset(clip, file);

        AssetDatabase.SaveAssets();
        return clip;
    }

    static void CreateAnimationController(List<AnimationClip> clipsList, string role)
    {
        string animatorControllerPath = $"{_animationControllerPath}/{role}/{role}_animator_ctrl.controller";

        AnimatorController animatorController =
            AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);

        // 添加参数
        animatorController.AddParameter(new AnimatorControllerParameter()
        {
            name = "Run",
            type = AnimatorControllerParameterType.Bool,
            defaultBool = false
        });

        AnimatorControllerLayer layer = animatorController.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;

        foreach (AnimationClip clip in clipsList)
        {
            AnimatorState state = sm.AddState(clip.name);
            state.motion = clip;
            state.writeDefaultValues = false;

            if (clip.name.Contains("idle"))
            {
                sm.defaultState = state;
            }

            // 设置参数 这里只设置了 不能过渡到自身 以及 过渡时间
            AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
            trans.canTransitionToSelf = false;
            trans.duration = 0;

            // 添加Animation条件
            trans.AddCondition(
                clip.name.IndexOf("run", StringComparison.Ordinal) > 0
                    ? AnimatorConditionMode.If
                    : AnimatorConditionMode.IfNot, 0,
                "Run");
        }

        AssetDatabase.SaveAssets();
    }
}