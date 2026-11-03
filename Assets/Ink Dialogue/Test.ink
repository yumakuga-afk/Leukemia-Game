// ============ Ink/NPC_Elder.ink ============
VAR task_id                = ""
VAR task_state             = 0
VAR has_required_items     = 0
VAR has_required_objects   = 0

=== talk ===
{ 
-task_state == 0:
    -> first_talk
- task_state == 1:
    -> repeat_talk
- else:
    -> done_talk
}

=== first_talk ===
# speaker: Old man
NPC: Hello, Traveler. I need some help.
# speaker: me
+ [How can I help you?]
    # speaker: Old man
    NPC: Help me find that old key, then go check the door at the ruins.
    ~ task_state = 1
    -> END
# speaker: me
+ [Maybe next time.]    
    # speaker: Old man
    NPC: Alright, come back anytime.
    -> END

=== repeat_talk ===
{ 
-has_required_items == 0 && has_required_objects == 0:
    NPC:You haven’t found the key yet, and you haven’t taken a look at the ruins either, have you?
- has_required_items == 0:
    NPC: The key hasn't been found yet.
- has_required_objects == 0:
    NPC: Remember to go take a look at the ruins.
- else:
    NPC: Have you taken care of everything?
}
-> END

=== done_talk ===
NPC: Thanks to you, everything has been resolved.
-> END