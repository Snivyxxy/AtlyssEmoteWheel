using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AtlyssEmotes
{
    public class EmoteOverrider : MonoBehaviour
    {
        public Animator anim;

        public ScriptableEmote emote;

        public bool play;

        public float fadeInTime;

        private PlayableGraph graph;
        private AnimationClipPlayable initPlayable;
        private AnimationClipPlayable loopPlayable;
        private AnimatorControllerPlayable contPlayable;
        private AnimationMixerPlayable channelmixer;
        private PlayableOutput playableOutput;
        private bool playing;


        void Start()
        {
            anim = GetComponent<Animator>();
            //Create Playable Graph

            graph = PlayableGraph.Create();

            //Make Mixer with 

            channelmixer = AnimationMixerPlayable.Create(graph, 3);

            //contPlayable = AnimatorControllerPlayable.Create(graph,anim.runtimeAnimatorController);
            channelmixer.ConnectInput(2, contPlayable, 0);

            playableOutput = AnimationPlayableOutput.Create(graph, "Animation", anim);

            playableOutput.SetSourcePlayable(channelmixer);

        }

        public void Stop()
        {
            playing = false;
            channelmixer.DisconnectInput(0);
            channelmixer.DisconnectInput(1);
            if (initPlayable.IsValid())
            {
                initPlayable.Destroy();
            }
            if (loopPlayable.IsValid())
            {
                loopPlayable.Destroy();
            }
            //channelmixer.SetInputWeight(2, 1f);
            channelmixer.SetInputWeight(0, 0f);
            channelmixer.SetInputWeight(1, 0f);
            graph.Stop();
        }
        void Play()
        {
            Stop();
            playing = true;
            if (emote.init == null && emote.loop != null)
            {
                Play(emote.loop, true);
            }
            else
            {
                Play(emote.init, false);
            }

        }

        void Play(AnimationClip clip, bool loopLayer)
        {
            AnimationClipPlayable p = loopLayer ? loopPlayable : initPlayable;
            if (p.IsValid())
            {
                p.Destroy();
            }

            p = AnimationClipPlayable.Create(graph, clip);

            if (!loopLayer)
            {
                p.SetDuration(clip.length);
            }

            p.SetTime(0d);

            channelmixer.ConnectInput(loopLayer ? 1 : 0, p, 0);



            if (loopLayer)
            {
                loopPlayable = p;
            }
            else
            {
                initPlayable = p;
            }
            playing = true;

            graph.Play();
        }
        void Update()
        {

            if (playing)
            {
                if(emote.init == null && emote.loop == null)
                {
                    Stop();
                }
                
                HandleWeights();

                if (initPlayable.IsValid()) //If Init Playable Is Active
                {
                    //Get InitPlayable's Normalized Time
                    double normalizedTime = GetNormalizedTime(initPlayable);
                    //If Loop Emote Isn't Null
                    //and Normalized Time is greater than the Emote End
                    if (emote.loop != null &&
                        normalizedTime >= Mathf.Clamp(emote.ExitTime - emote.TransitionOffset, 0, 1) &&
                        !loopPlayable.IsValid())
                    {
                        Play(emote.loop, true);
                    }
                    else if (emote.loop == null && initPlayable.IsDone())
                    {
                        Stop();
                    }
                }
            }
        }
        
        

        private void HandleWeights()
        {
            float initWeight;
            float loopWeight;
            float emoteWeight;

            Playable p = initPlayable.IsValid() ? initPlayable : loopPlayable;

            double clipTime = p.GetTime();
            double clipDuration = initPlayable.IsValid() ? initPlayable.GetDuration() : emote.loop.length;
            double normalizedTime = clipTime / clipDuration;

            //init crossfade handler
            if (fadeInTime == 0)
            {
                emoteWeight = 1f;
            }
            else
            {
                emoteWeight = Mathf.Clamp((float)(normalizedTime / fadeInTime), 0f, 1f);
            }

            //Crossfade Types
            
            if (emote.init == null) //Loop No Intro
            {
                loopWeight = 1f;
                initWeight = 0f;
            }
            else //init is not null here.
            {
                bool loopPlaysThisFrame = false;
                //Crossfade Handler
                if (normalizedTime < emote.ExitTime) //if init animation's time is before crossfade time
                {
                    initWeight = 1f;
                    loopWeight = 0f;
                }
                else if (normalizedTime >= Mathf.Clamp(emote.ExitTime + emote.TransitionDuration, 0, 1)) //if init animation's time is after crossfade time Or init is finished.
                {
                    initWeight = 0f;
                    loopWeight = 1f;
                    loopPlaysThisFrame = true;
                }
                else //If init animation's time is between crossfade time
                {
                    //transition Loopweight from 0 to 1 over Transition duration
                    //0 is when normalized time is exittime
                    //1 is when normalized time is exittime + transition duration
                    loopWeight = (float)((normalizedTime - emote.ExitTime) / emote.TransitionDuration); 
                    
                    initWeight = (float)(1f - loopWeight);
                }
                //Check Crossfade in NonLoopings
                if (!(loopPlayable.IsValid() || loopPlaysThisFrame))
                {
                    emoteWeight = initWeight;
                    initWeight = 1f;
                    loopWeight = 0f;
                }

            }
            
            channelmixer.SetInputWeight(0, initWeight * emoteWeight);
            channelmixer.SetInputWeight(1, loopWeight * emoteWeight);
            //channelmixer.SetInputWeight(2, 1f - emoteWeight);
            /*
            Plugin.Logger.LogInfo("Init  Weight:" + initWeight);
            Plugin.Logger.LogInfo("Loop  Weight:" + loopWeight);
            Plugin.Logger.LogInfo("Emote Weight:" + emoteWeight);
            Plugin.Logger.LogInfo("Loop  Valid :" + loopPlayable.IsValid());
            Plugin.Logger.LogInfo("Emote Valid :" + initPlayable.IsValid());
            Plugin.Logger.LogInfo("ClipTime    :" + normalizedTime);
            */


        }

        double GetNormalizedTime(Playable p, bool last = false)
        {
            double initTime = last ? p.GetPreviousTime() : p.GetTime();
            double initDuration = p.GetDuration();
            return initTime / initDuration;
        }


        
        void OnDestroy()
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }
        }
        
        [HarmonyPatch]
        static class Patches
        {
            [HarmonyPatch(typeof(PlayerVisual), "Local_CrossFadeAnim")]
            [HarmonyPostfix]
            static void PlayCustomEmotes(PlayerVisual __instance, string _animName, float _animFadeTime, int _animLayer)
            {
                if(__instance == null ||
                   __instance._visualAnimator == null)
                {
                    return;
                }                
                EmoteOverrider extraEmotes = __instance._visualAnimator.GetComponent<EmoteOverrider>();
                if (_animLayer == 11 &&
                    EmoteManager.allEmotes.ContainsKey(_animName) &&
                    !EmoteManager.allEmotes[_animName].isVanilla &&
                    extraEmotes != null)
                {
                    extraEmotes.emote = EmoteManager.allEmotes[_animName];
                    extraEmotes.Play();
                    //PlayEmote
                    extraEmotes.fadeInTime = _animFadeTime;
                }
            }


            
            [HarmonyPatch(typeof(PlayerVisual), "Iterate_AnimationCallback")]
            [HarmonyPostfix]
            
            static void StopEmotingAtPoints(PlayerVisual __instance, string _animName, int _animLayer)
            {
                EmoteOverrider extraEmotes = __instance._visualAnimator.GetComponent<EmoteOverrider>();
                if (extraEmotes == null)
                {
                    return;
                }

                switch (_animLayer)
                {
                    case 0:
                        if (_animName != "Idle" && _animName != "Idle_02")
                        {
                            extraEmotes.Stop();
                        }
                        return;
                    case 1:
                        if (_animName != "Empty")
                        {
                            extraEmotes.Stop();
                        }
                        break;
                    case 3:
                        if (_animName != "Empty")
                        {
                            extraEmotes.Stop();
                        }
                        return;
                    case 7:
                        if (_animName != "Empty")
                        {
                            extraEmotes.Stop();
                        }
                        return;
                    case 8:
                        if (_animName != "Empty")
                        {
                            extraEmotes.Stop();
                            return;
                        }
                        return;
                    case 9:
                        extraEmotes.Stop();
                        break;
                    case 10:
                        if (_animName == "Dash")
                        {
                            extraEmotes.Stop();
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
        }
        

    }
}