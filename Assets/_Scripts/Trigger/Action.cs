using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace TGS
{
	[CustomPropertyDrawer(typeof(Action))]
	public class ActionDrawer : PropertyDisplay
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position = DrawBaseProperty(position, property);
			if (!property.isExpanded)
			{
				return;
			}
			DrawBackgroundRect(position);
			SerializedProperty type = property.FindPropertyRelative("type");
			Action.Type t = (Action.Type)type.enumValueIndex;
			Rect sp = new Rect(position);
			sp = DrawSubProperty(sp, type);

			switch (t)
			{
				case Action.Type.pathfindToTransform:
				{
					SerializedProperty flightAI = property.FindPropertyRelative("flightAI");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty speed = property.FindPropertyRelative("speed");
					sp = DrawSubProperty(sp, flightAI);
					sp = DrawSubProperty(sp, transform);
					sp = DrawSubProperty(sp, speed);
				}
				break;

				case Action.Type.spawnEffectAtTransform:
				{
					SerializedProperty effect = property.FindPropertyRelative("effect");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty size = property.FindPropertyRelative("size");
					SerializedProperty magnitude = property.FindPropertyRelative("magnitude");
					sp = DrawSubProperty(sp, effect);
					sp = DrawSubProperty(sp, size);
					sp = DrawSubProperty(sp, magnitude);
					sp = DrawSubProperty(sp, transform);
				}
				break;

				case Action.Type.deactivatePuzzleNode:
				case Action.Type.activatePuzzleNode:
				{
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					sp = DrawSubProperty(sp, puzzleNode);
				}
				break;

				case Action.Type.waitForPuzzleNode:
				{
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					sp = DrawSubProperty(sp, puzzleNode);
				}
				break;

				case Action.Type.waitForSeconds:
				{
					SerializedProperty seconds = property.FindPropertyRelative("seconds");
					sp = DrawSubProperty(sp, seconds);
				}
				break;

				case Action.Type.commsMessage:
				{
					//trigger.comms.Message(trigger, persona, dialog, idleSeconds, transform);
					SerializedProperty persona = property.FindPropertyRelative("persona");
					SerializedProperty dialog = property.FindPropertyRelative("dialog");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty idleSeconds = property.FindPropertyRelative("idleSeconds");
					sp = DrawSubProperty(sp, persona);
					sp = DrawSubProperty(sp, transform);
					sp = DrawSubProperty(sp, idleSeconds);
					sp = DrawSubProperty(sp, dialog);
				}
				break;

				case Action.Type.indicate:
				{
					SerializedProperty worldPos = property.FindPropertyRelative("position");
					SerializedProperty instruction = property.FindPropertyRelative("instruction");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					SerializedProperty objectiveMarker = property.FindPropertyRelative("objectiveMarker");
					sp = DrawSubProperty(sp, worldPos);
					sp = DrawSubProperty(sp, instruction);
					sp = DrawSubProperty(sp, transform);
					sp = DrawSubProperty(sp, puzzleNode);
					sp = DrawSubProperty(sp, objectiveMarker);
				}
				break;

				default:
					break;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isExpanded)
			{
				return MeasureBaseProperty(property);
			}
			SerializedProperty type = property.FindPropertyRelative("type");
			Action.Type t = (Action.Type)type.enumValueIndex;

			float res = MeasureBaseProperty(property) + MeasureSubProperty(type) + 4;

			switch (t)
			{
				case Action.Type.pathfindToTransform:
				{
					SerializedProperty flightAI = property.FindPropertyRelative("flightAI");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty speed = property.FindPropertyRelative("speed");
					res += MeasureSubProperty(flightAI);
					res += MeasureSubProperty(transform);
					res += MeasureSubProperty(speed);
				}
				break;

				case Action.Type.spawnEffectAtTransform:
				{
					SerializedProperty effect = property.FindPropertyRelative("effect");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty size = property.FindPropertyRelative("size");
					SerializedProperty magnitude = property.FindPropertyRelative("magnitude");
					res += MeasureSubProperty(effect);
					res += MeasureSubProperty(size);
					res += MeasureSubProperty(magnitude);
					res += MeasureSubProperty(transform);
				}
				break;

				case Action.Type.activatePuzzleNode:
				{
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					res += MeasureSubProperty(puzzleNode);
				}
				break;

				case Action.Type.waitForPuzzleNode:
				{
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					res += MeasureSubProperty(puzzleNode);
				}
				break;

				case Action.Type.waitForSeconds:
				{
					SerializedProperty seconds = property.FindPropertyRelative("seconds");
					res += MeasureSubProperty(seconds);
				}
				break;

				case Action.Type.commsMessage:
				{
					//trigger.comms.Message(trigger, persona, dialog, idleSeconds, transform);
					SerializedProperty persona = property.FindPropertyRelative("persona");
					SerializedProperty dialog = property.FindPropertyRelative("dialog");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty idleSeconds = property.FindPropertyRelative("idleSeconds");
					res += MeasureSubProperty(persona);
					res += MeasureSubProperty(dialog);
					res += MeasureSubProperty(transform);
					res += MeasureSubProperty(idleSeconds);
				}
				break;

				case Action.Type.indicate:
				{
					SerializedProperty position = property.FindPropertyRelative("position");
					SerializedProperty instruction = property.FindPropertyRelative("instruction");
					SerializedProperty transform = property.FindPropertyRelative("transform");
					SerializedProperty puzzleNode = property.FindPropertyRelative("puzzleNode");
					SerializedProperty objectiveMarker = property.FindPropertyRelative("objectiveMarker");
					res += MeasureSubProperty(position);
					res += MeasureSubProperty(instruction);
					res += MeasureSubProperty(transform);
					res += MeasureSubProperty(puzzleNode);
					res += MeasureSubProperty(objectiveMarker);
				}
				break;

				default:
					break;
			}

			return res;
		}
	}
}
#endif

namespace TGS
{ 
	[System.Serializable]
	public class Action
	{
		public enum Type
		{
			pathfindToTransform,
			spawnEffectAtTransform,
			activatePuzzleNode,
			deactivatePuzzleNode,
			waitForPuzzleNode,
			waitForSeconds,
			commsMessage,
			indicate
		}

		public Type type;
		public PuzzleNode puzzleNode;
		public ObjectiveMarker objectiveMarker;
		public FlightAI flightAI;
		public Transform transform;
		public Persona persona;
		[TextArea]
		public string dialog;
		public string instruction;
		public SFX effect;
		public float speed = 1;
		public float idleSeconds = 0.8f;
		public float seconds = 1;
		public float timeout = 0;
		public float size = 1;
		public float magnitude = 1;
		public Vector2 position;

		public void Act(Trigger trigger, GameObject origin)
		{
			switch (type)
			{
				case Type.pathfindToTransform:
					if (flightAI != null)
					{
						flightAI.Pathfind(transform.position, speed);
					}
					break;

				case Type.spawnEffectAtTransform:
					if (effect != null)
					{
						SFX e = SFX.Spawn(effect, transform);
						e.size = size;
						e.magnitude = magnitude;
						e.normal = transform.forward;
					}
					break;

				case Type.activatePuzzleNode:
				case Type.deactivatePuzzleNode:
					if (puzzleNode != null)
					{
						puzzleNode.SetCompletion(type == Type.activatePuzzleNode);
					}
					break;

				case Type.waitForPuzzleNode:
					break;

				case Type.waitForSeconds:
					timeout = seconds + Time.time;
					break;

				case Type.commsMessage:
					if (trigger != null && trigger.comms != null)
					{
						trigger.comms.Message(trigger, persona, dialog, idleSeconds, transform);
					}
					break;

				case Type.indicate:
				{
					if (puzzleNode != null && instruction != null && objectiveMarker != null)
					{
						NoteControl.StaticIndicate(position, instruction, transform, puzzleNode, objectiveMarker);
					}
				}
				break;

				default:
					break;
			}
		}

		public bool Ready(Trigger trigger)
		{
			switch (type)
			{
				case Type.waitForPuzzleNode:
					if (puzzleNode)
						return puzzleNode.complete;
					break;

				case Type.waitForSeconds:
					return Time.time >= timeout;

				case Type.commsMessage:
					if (trigger != null && trigger.comms != null)
					{
						return trigger.comms.Ready(trigger);
					}
					break;
				default:
					break;
			}
			return true;
		}
	}
}