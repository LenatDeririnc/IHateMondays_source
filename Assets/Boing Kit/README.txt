Boing Kit - Bouncy VFX Tools for Unity

Publisher:
  Long Bunny Labs
    LongBunnyLabs@gmail.com
    http://LongBunnyLabs.com

Examples:
  For step-by-step instructions on how to set up effects demonstrated in individual examples, 
  please refer to the README.txt file located in each of the example folder.

Tooltips:
  If you need details on a specific component property, 
  please hover the mouse cursor over the property in the editor's inspector, 
  which will cause the tooltip for the property to who up.

If you run into any issues with Boing Kit, please feel free to email me at LongBunnyLabs@gmail.com.
I'd be more than happy to help.


---------------------------------------------------------------------------------------------------

Overview & Quick Start:

  [Shared Boing Params]

    This is a data structure that contains parameters that Boing Kit uses to create bouncy effects. 

    For example, it contains parameters like oscillation frequency and decay rate.

    Each component Boing Kit provides can either use its own local parameters, or use a 
    Shared Boing Params asset created by right clicking in the Project window, and then select 
    Create > Boing Kit > Shared Boing Params.


  [Boing Behavior]

    Add this component to make objects bouncy in reaction to position & rotation change.

    Any change to an object's transform (position & rotation) during its Update function will be 
    picked up by this component and used to create bouncy effects during LateUpdate.

    For example, once this component is added to an object that can be dragged by the mouse, 
    the object will lag behind the mouse cursor as if there's a spring attached between 
    the object and the mouse cursor.


  [Boing Effector]

    Add this component to affect game objects with Boing Reactor components. 

    See the Boing Reactor section below for furtherinformation.


  [Boing Reactor]
    
    Add this component to be affected by game objects with Boing Effector components.

    Once this component is added to an object, the object will not only exhibit behaviors from 
    the Boing Behavior component, but will also be affected by nearby objects with the 
    Boing Effector component.


  [Boing Reactor Field]

    Add this component to create a "proxy grid" of reactors affected by effectors.

    The reactor field component is an alternative to making effectrs affect objects, 
    and is an optimization for certain use cases. The trade-off is that the reactor 
    field only works within a limited area defined by a grid's dimensions and cell size.

    This is good for making effects affect a large amount of objects with the
    Boing Reactor Field sampler component (mentioned below).
    Also, reactor field can be sampled by shaders (powered by the GPU).

    Before starting to play with reactor field propagation, please make sure you are familiar 
    with the basics of how effectors and reactor fields interact first.


  [Boing Reactor Field Sampler]

    Add this component to sample from a reactor field and be affected by effectors.

    Instead of directly being affected by effectors, reactor field sampler works by 
    letting the "proxy grid" of reactors in the reactor field be affected by the effectors, 
    and then sampling from the field to apply the effects to the objects with samplers.

    If using the GPU sampler on an object, it must use a material with a shader that calls the 
    ApplyBoingReactorFieldPerObject or ApplyBoingReactorFieldPerVertex shader functions in the 
    vertex shader. Boing Kit comes with example shaders that already properly call these functions, 
    modified from Unity's standard shader. To see how to call these functions in custom vertex 
    shaders, please check out the Example Custom Shader files in the Warped Teapots example.


  [Boing Bones]

    Add this component to create bouncy hierarchies of skeletal bones or object transforms.

    The very first step is to specify the root transform of a bone chain. The bouncy effects will 
    begin at the root and then propagate throughout the entire hierarchy.

    Multiple chains, each having its own root, can be specified for each Bouncy Bones component.

    Boing Bones can react to lightweight colliders provided by Boing Kit added to the 
    Boing Colliders section in the inspector. Boing Bones can also react to Unity's standard 
    colliders added to the Unity Collider section.

    Boing Bones can also react to Boing Effectors, just like Boing Reactors.


---------------------------------------------------------------------------------------------------

FAQ

  Q: The effects on the grass field and explosion/implosion field in Boing Kit's examples 
     are not working.

  A: This is due to an issue with certain versions of Unity, where material property blocks passed 
     in for instanced mesh rendering are not respected. This has been fixed since Unity 2018.3.

  --

  Q: Boing bones doesn't seem to work well with other procedural animation assets, like Final IK.

  A: Proceduarl animation assets, including Boing Kit, update during the LateUpdate phase.
     There is no guaranteed update order of procedural animation assets unless specified.
     Boing Kit's update needs to happen after all other procedural animation assets.
     This can be set up via the Unity option: Edit > Project Settings > Script Execution Order.

  --

  Q: Materials in the examples are not rendered properly under scriptable render pipeline (SRP), 
     including Unity's LWRP and HDRP.

  A: Materials included in the examples are targeted at the standard render pipeline.
     It is recommended that you try out the examples under Unity's standard render pipeline first.
     In order for GPU-based reactor field samplers to work in scriptable render pipelines, 
     you'd have to call the shader functions provided by Boing Kit in your vertex shaders, 
     as shown in the example shaders.
     The functions are ApplyBoingReactorFieldPerObject and ApplyBoingReactorFieldPerVertex.
     For usage examples, please check out the Example Custom Shader files under the Warped Teapots example.

---------------------------------------------------------------------------------------------------

Changes:
  Version 1.2.37:
   - Disable broken attempt at altering BoingBone.TwistPropagaion to match default values on older versions.
     Somehow Unity always calls BoingBase.OnAfterDeserialize with a revision of 0 for the first time.

  Version 1.2.36:
   - Fix incorrect FixedUpdate implementation. Now FixedUpdate should update with the rest of the game's FixedUpdate as expected.
   - Consolidate update mode and update timing into 3 update modes: FixedUpdate, Early Update, and Late Update (default).
     NOTE: projects using older Boing Kit versions will need to re-adjust this property after updating!
   - Remove more calls to GC.Alloc.

  Version 1.2.35:
   - Fix boing behaviors & reactors with non-unit scales rebooted to unit scales.

  Version 1.2.34:
   - Add scale effects to boing behaviors (off by default).
   - Fix changes to bone scales not picked up by the Boing Bones component.
   - Fix null reference exceptions for effector parameter list.

  Version 1.2.33:
   - Add Twist Propagation property to the Boing Bones component (defaulted to true for new creations; defaulted to false for older creations for backward compatibility).
   - Version tracking.

  Version 1.2.32:
   - Fix missing rotation propagation down bone chains.

  Version 1.2.31:
   - Smarter chain rescan condition checks that take into account of hierarchy change.

  Version 1.2.30:
   - Fix false abortion of run-time chain rescan.
   - Fix mismatched delta time used for applying gravity to bones.

  Version 1.2.29:
   - Fix garbage collection upon calling Aabb.FromPoints.
   - Fix garbage collection upon calling VectorUtil.ComponentWiseMult.

  Version 1.2.28:
   - Fix garbage collection upon calling Bits32.IsBitSet.

  Version 1.2.27:
   - Restore update timing. (Unity should really fix the timing of their sprite framework's transform read; it should be after LateUpdate(), NOT after Update())
   - Fix one-frame-off error on non-loose root bones.

  Version 1.2.26:
   - Fix reactor field compute shader for DirectX.
   - Fix time dependence of effector accumulation.

  Version 1.2.25:
   - Fix inspector header text color in dark theme.

  Version 1.2.24:
   - Remove update timing option, as it was a crutch that introduced too much code complexity and performance hit, all to accommodate issues in other external systems that improperly pull transform data too early for rendering.
   - Fix jitter in fixed update mode.

  Version 1.2.23:
   - Add shader subgraph, which can be used to create URP & HDRP shader graphs that sample reactor fields.

  Version 1.2.22:
   - Fix boing bones jitter in builds.

  Version 1.2.21:
   - Fix boing bones not restoring to original bone positions upon being disabled.
   - Fix objects reset to identity transform on synchronous scene reload.

  Version 1.2.20:
   - Fix exception upon adding the boing bones component.
   - Fix bone exclusion from the boing bones component.

  Version 1.2.19:
   - Fix boing behaviors & reactors having rotation permanently changed without properly restoring when outside influence is gone.

  Version 1.2.18:
   - Fix editor errors when editing boing reactor fields in inspector.
   - Fix inability to move objects with boing behaviors in editor under play mode.

  Version 1.2.17:
   - Use script execution order instead of camera-based events to apply and restore transforms. If this breaks interaction between Boing Kit and other procedural animation scripts, please simply re-adjust their script execution order. Typically, Boing Kit's post-update pump needs to be last in the execution order list and pre-update pump needs to be first in the list.
   - Add update timing (Early v.s. Late) to boing beahviors, boing reactors, boing reactor field CPU samplers, and boing bones. This option dictates when to apply transforms for rendering. The Late option is the original and default, which apply transforms for rendering at the end of LateUpdate. The Early applies transforms for rendering at the end of Update, which is for accommodating scripts that unexpectedly pull transforms for rendering between Update and LateUpdate, such as Unity's skinned sprite renderer in certain versions.

  Version 1.2.16:
   - Add update mode (Update v.s. FixedUpdate) to boing behaviors and boing reactors. Use matching update mode to other game logic to avoid potential jitter.
   - Add options to lock translational effects along certain axes (in either global or local space).

  Version 1.2.15:
   - Fix issues with gravity in fixed-update mode.

  Version 1.2.14:
   - Add update mode (Update v.s. FixedUpdate) to boing bones. Use matching update mode to other game logic to avoid potential jitter.
   - Fix boing bones issues with switching/reloading scenes.
   - Fix bone transform drifts over prolonged period of time.

  Version 1.2.13:
   - Fix bones becoming further apart over time.

  Version 1.2.12:
   - Fix issues with multiple cameras in game & multiple views in editor.
   - Add README files that contain a new overview, quick start guide, and instructions on how to set up effects demonstrated in some examples.

  Version 1.2.11:
   - Fix namespace conflicts with Oculus SDK in examples.

  Version 1.2.10:
   - Fix bad child rotation under bones with multiple children.

  Version 1.2.9:
   - Use a globally shared dummy collider for all boing bones.

  Version 1.2.8:
   - Fix crash upon exceeding initial effector count limit.

  Version 1.2.7:
   - Fix profiler error messages.

  Version 1.2.6:
   - Fix for multiple cameras.
   - NOTE: Due to technical limitations, effects will only appear in one single active Edit or Game view under Play Mode in the Unity editor (whichever gets rendered first). However, effects should show up normally in all cameras in built games.

  Version 1.2.5:
   - SRP support (including LWRP & HDRP) for Unity 2019.1 or newer.
   - NOTE: Materials included in the examples are still targeted for the standard render pipeline. If using LWRP or HDRP, they would need to be upgraded to the scriptable render pipelines via the menu items under Edit > Render Pipeline. However, the example materials used alongside the reactor field's compute shader are modified from Unity's standard materials (hence non-standard) and cannot be automatically upgraded for LWRP or HDRP. It is recommended that you try out all the examples under Unity's standard render pipeline.

  Version 1.2.4:
   - Dynamic bouncy bones.

  Version 1.1.3:
   - Reactor field propagation.

  Version 1.0.2:
   - Registry callbacks.

  Version 1.0.1:
   - Fix for custom shader example.

  Version 1.0.0:
   - Initial release.
  Original

