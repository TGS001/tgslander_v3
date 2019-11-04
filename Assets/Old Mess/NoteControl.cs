using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteControl : MonoBehaviour {
    public ScoreNote scoreNote;
    public Indicator regularIndicator;

	public void SendScoreNote(Vector2 position, string message, int points) {
        ScoreNote note = Instantiate(scoreNote);
        note.message = message;
        note.points = points;
        note.transform.position = position;
    }

    public void Indicate(Vector2 position, string instruction, Transform target, PuzzleNode node, ObjectiveMarker marker) {
        Indicator ind = Instantiate(regularIndicator);
        ind.instruction = instruction;
        ind.target = target;
        ind.node = node;
        ind.marker = marker;
        ind.transform.position = position;
    }

    public static void StaticIndicate(Vector2 position, string instruction, Transform target, PuzzleNode node, ObjectiveMarker marker) {
        NoteControl nc = FindObjectOfType<NoteControl>();
        nc.Indicate(position, instruction, target, node, marker);
    }
}
