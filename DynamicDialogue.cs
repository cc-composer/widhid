using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDialogue : MonoBehaviour
{
    public GameObject player;

    private uint eObjectiveStatus = AkSoundEngine.GetIDFromString("Objective_Status");
    private uint eUnitUnderAttack = AkSoundEngine.GetIDFromString("Unit_Under_Attack");
    private uint eWalkieTalkie = AkSoundEngine.GetIDFromString("WalkieTalkie");
    private AkPlaylist pPlaylist = null;

    [Header("States")]
    [SerializeField] AK.Wwise.State sUnit;
    [SerializeField] AK.Wwise.State sObjective;
    [SerializeField] AK.Wwise.State sObjectiveStatus;
    [SerializeField] AK.Wwise.State sHostile;
    [SerializeField] AK.Wwise.State sLocation;
    [SerializeField] AK.Wwise.State sWalkieTalkie;

    // Start is called before the first frame update
    void Start()
    {
        DynamicDialogueA();
    }

    private void DynamicDialogueA()
    {       
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        uint[] aWalkieTalkie = new uint[1] { sWalkieTalkie.Id };
        uint[] aObjectiveStatus = new uint[3] {
                                                sUnit.Id,
                                                sObjective.Id,
                                                sObjectiveStatus.Id
                                              };
        uint[] aUnitUnderAttack = new uint[3] {
                                                sUnit.Id,
                                                sObjective.Id,
                                                sObjectiveStatus.Id
                                              };

        uint nodeID = AkSoundEngine.ResolveDialogueEvent(eWalkieTalkie, aWalkieTalkie, 1);
        pPlaylist.Enqueue(nodeID);

        nodeID = AkSoundEngine.ResolveDialogueEvent(eObjectiveStatus, aObjectiveStatus, 3);
        pPlaylist.Enqueue(nodeID);

        nodeID = AkSoundEngine.ResolveDialogueEvent(eWalkieTalkie, aWalkieTalkie, 1);
        pPlaylist.Enqueue(nodeID);

        AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        AkSoundEngine.DynamicSequencePlay(sequenceID);
        StartCoroutine(Wait(sequenceID));
    }
    private void AddItemToPlaylist(uint sequenceID)
    {
        pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
        if (pPlaylist != null)
        {
            print("Loading playlist.");
            //Add new item in the playlist during the playback of the dynamic sequence.
            uint[] aWalkieTalkie = new uint[1] { sWalkieTalkie.Id };
            uint[] aUnitUnderAttack = new uint[3] {
                                                sUnit.Id,
                                                sHostile.Id,
                                                sLocation.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(eUnitUnderAttack, aUnitUnderAttack, 3);
            pPlaylist.Enqueue(nodeID);

            nodeID = AkSoundEngine.ResolveDialogueEvent(eWalkieTalkie, aWalkieTalkie, 1);
            pPlaylist.Enqueue(nodeID);

            //Unlock the playlist ASAP to prevent problems.
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkSoundEngine.DynamicSequencePlay(sequenceID);
            AkSoundEngine.DynamicSequenceClose(sequenceID);
        }
    }

    IEnumerator Wait(uint sequenceID)
    {
        yield return new WaitForSeconds(1f);
        AddItemToPlaylist(sequenceID);
    }

    void SimpleSequence()
    {
        //Get the ID of the Dynamic Dialogue Event
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");

        //Get the ID of the player and open a Dynamic Sequence on them.
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        //You can change the open setting to NormalTransition to prevent the next sound in the playlist from being prepared
        //in advance and removed from the playlist before the current sound finishes playing.
        //uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player, 0, null, null, AkDynamicSequenceType.DynamicSequenceType_NormalTransition);

        //Lock the playlist for editting
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        //Make an argument path by checking the ID of both states.
        uint[] argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };

        //Make a new ID to resolve the Dialogue event.
        uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
        pPlaylist.Enqueue(nodeID);
        //When enqueuing items, you can add a value at the end to indicate a delay in milliseconds.
        //pPlaylist.Enqueue(nodeID, 300)

        AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        
        //Play the Dynamic Sequence
        AkSoundEngine.DynamicSequencePlay(sequenceID);

        //Close the Dynamic Sequence. Note, it will continue to play until the end, but you won't be able to modify it anymore.
        AkSoundEngine.DynamicSequenceClose(sequenceID);
    }

    void AddItemToPlaylist()
    {
        //Repeat Simple Sequence, but do not close.
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        if (pPlaylist != null)
        {
            uint[] argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            pPlaylist.Enqueue(nodeID);
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkSoundEngine.DynamicSequencePlay(sequenceID);
        }

        //Lock the playlist to add item during playback
        // NOTE: You should not keep the playlist locked for too long when the sequence is playing.
        // It could could result in a stall in the Sound Engine.
        pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
        if (pPlaylist != null)
        {
            //Add new item in the playlist during the playback of the dynamic sequence.
            uint[] argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            pPlaylist.Enqueue(nodeID);
            
            //Unlock the playlist ASAP to prevent problems.
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkSoundEngine.DynamicSequencePlay(sequenceID);
            AkSoundEngine.DynamicSequenceClose(sequenceID);
        }
    }

    void InsertItemToListDuringPlayback()
    {
        //Put multiple items into the playlist and play.
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        if(pPlaylist != null)
        {
            uint[] argPath = new uint[2] { sWalkieTalkie.Id,
                                       sWalkieTalkie.Id };

            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            argPath[0] = sWalkieTalkie.Id;
            argPath[1] = sWalkieTalkie.Id;

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            argPath[0] = sWalkieTalkie.Id;
            argPath[1] = sWalkieTalkie.Id;

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        }

        AkSoundEngine.DynamicSequencePlay(sequenceID);

        //
        //
        //

        //Here we will add items to the front or middle of the playlist during playback.
        //Lock the playlist to add items during playback.
        pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        if (pPlaylist != null)
        {
            uint[] argPath = new uint[2] { sWalkieTalkie.Id,
                                           sWalkieTalkie.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            //Insert a new item in front of the playlist
            //Make sure to initialize all members of the PlaylistItem struct
            AkPlaylistItem pPlaylistItem = pPlaylist.Insert(0);
            pPlaylistItem.audioNodeID = nodeID;
            pPlaylistItem.msDelay = 0;
            pPlaylistItem.pCustomInfo = System.IntPtr.Zero;

            argPath = new uint[2] { sWalkieTalkie.Id,
                                    sWalkieTalkie.Id };

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            //Insertion in the middle of the playlist
            pPlaylistItem = pPlaylist.Insert(1);
            pPlaylistItem.audioNodeID = nodeID;
            pPlaylistItem.msDelay = 0;
            pPlaylistItem.pCustomInfo = System.IntPtr.Zero;

            argPath = new uint[2] { sWalkieTalkie.Id,
                                    sWalkieTalkie.Id };

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            //Insertion in the middle of the playlist
            pPlaylistItem = pPlaylist.Insert(2);
            pPlaylistItem.audioNodeID = nodeID;
            pPlaylistItem.msDelay = 0;
            pPlaylistItem.pCustomInfo = System.IntPtr.Zero;

            argPath = new uint[2] { sWalkieTalkie.Id,
                                    sWalkieTalkie.Id };

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        }

        AkSoundEngine.DynamicSequenceClose(sequenceID);
    }

    void AddItemToEmptyListDuringPlayback()
    {
        //Start the playback right away. We will add new items directly in the already playing dynamic sequence.
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        AkSoundEngine.DynamicSequencePlay(sequenceID);

        //
        //
        //
        
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        if (pPlaylist != null)
        {
            uint[] argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            pPlaylist.Enqueue(nodeID);
            
            //As soon as you unlock the playlist, the new items will start to play.
            //Note: Do not keep the playlist locked after the insertion!
            //To trigger the playback at the right time, stop the dynamic sequence before adding the item and play it at the right time.
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);

            //Item added, go to step 3
        }

        //
        //
        //

        //Wait until the playlist is empty, at which point the last item will be playing.
        //Then add new items that will start to play right away.
        //NOTE: The newly added items are played with sample accuracy if the last item is still playing.
        pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);
        if (pPlaylist != null && pPlaylist.IsEmpty())
        {
            //The playlist is empty. We can now add new items and they will play right away.
            uint[] argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };
            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            pPlaylist.Enqueue(nodeID);

            argPath = new uint[2] { sWalkieTalkie.Id, sWalkieTalkie.Id };
            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            pPlaylist.Enqueue(nodeID);

            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkSoundEngine.DynamicSequenceClose(sequenceID);
        }
        else if (pPlaylist != null)
        {
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        }
    }

    void UsingDifferentCalls()
    {
        //Put multiple items into the playlist and play.
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        if (pPlaylist != null)
        {
            uint[] argPath = new uint[2] { sWalkieTalkie.Id,
                                       sWalkieTalkie.Id };

            uint nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            argPath[0] = sWalkieTalkie.Id;
            argPath[1] = sWalkieTalkie.Id;

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);

            argPath[0] = sWalkieTalkie.Id;
            argPath[1] = sWalkieTalkie.Id;

            nodeID = AkSoundEngine.ResolveDialogueEvent(myID, argPath, 2);
            AkSoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        }

        AkSoundEngine.DynamicSequencePlay(sequenceID);

        //
        //
        //

        //Stop the playback immediately.
        AkSoundEngine.DynamicSequenceStop(sequenceID);
        //OR AkSoundEngine.DynamicSequenceBreak(sequenceID); which will allow the audio to completely finish.
        //OR AkSoundEngine.DynamicSequencePause(sequenceID); which will pause and resume the audio at the exact point.
        //AkSoundEngine.DynamicSequenceResume(sequenceID)

        //
        //
        //

        //Play the rest of the sequence.
        //Note: The sequence will restart with the item that was in the playlist after the item that was stopped.
        AkSoundEngine.DynamicSequencePlay(sequenceID);
        AkSoundEngine.DynamicSequenceClose(sequenceID);
    }

    void PlaylistRemovals()
    {
        uint myID = AkSoundEngine.GetIDFromString("TestDialogueEvent");
        uint sequenceID = AkSoundEngine.DynamicSequenceOpen(player);
        AkPlaylist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        AkPlaylistItem pPlaylistItem = pPlaylist.ItemAtIndex(0);
        pPlaylist.Remove(pPlaylistItem);
        pPlaylist.RemoveAll();
        pPlaylist.RemoveLast();
        pPlaylist.RemoveSwap(pPlaylistItem);

    }
}
