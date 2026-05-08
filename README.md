# Join the new Lethal Upgrades Program!

## Currently Offering

### Player Upgrades

"We heard some employees could not complete their quotas, complaining and exclaiming
"oh it's just not fair!" blah blah...

After we fed them to **[THE COMPANY]** though, we got to thinking... \
What if we DID give them upgrades? \
And now here we are!

They come at a cost, but we can offer you the chance to **[FEED HIM MORE]**
while powering you up!" \
Seems fair... *right?*

**Health** - Take a few more thumper hits!
- Tier 1: Gain +20 additional health.
- Tier 2: Reduce all incoming damage by 10%. 
- Tier 3: Gain +30 additional health. 
- Legendary: Gain an adaptive regeneration ability.
    + If you are below 50% health, gain +0.5 hp/s regen until back up to 50%.
    + If you are below 100% but above 50% health and have not taken damage again 20 seconds after the first time, gain a passive + 2 hp/s buff until back up to 100%. Can be stopped if taking damage again.

**Stamina** - More items carried, more **[FEEDING IT]**
- Tier 1: Decrease running stamina usage. 
- Tier 2: Improve stamina regen by 10%. 
- Tier 3: Greatly reduced stamina penalties when heavy (>=50 lbs). 
- Legendary: When damaged, regardless of amount or source, gain full stamina back. 

**Movement** - Run away from that vile giant.
- Tier 1: Sprint 6% faster.
- Tier 2: Walk/Crouch 10% faster.
- Tier 3: Increased jump height by 25%.
- Legendary: While critically injured, gain movement speed and intangibility.
    + Always gain 1.5x more movement speed.
    + Become invisible to some enemies and invulnerable to all.
        - Has a 120 second cooldown after recovering from critical health.

**Utility** - They might be useful.
- Tier 1: Increase flashlight battery capacities by 10%.
- Tier 2: Shovels deal double damage. Explodes on hit as visual effect. 
    + By default, explosion does no damage. Only shovel deals damage.
    + To turn explosion visual on or off, type `shovel explosion`.
    + If explosion is on, you can also toggle off or on shovel jumping!
        - At the cost of 2hp, elevate yourself with your shovel quite the height!
        - If your teammate has shovel jump on, he wil also be affected, just... be careful where you send him.
        - Type `shovel jump` in terminal to toggle it on or off. 
- Tier 3: All equipment weighs 0 pounds. Cost: $400
- Legendary: Unlock 1 weather re-roll for any moon you orbit. Resets after leaving a moon. 
    + Grants you one re-roll to change the weather of the moon you are orbiting!
    + Once you use it, it will only reset after you land on a moon and come back to orbit.
    + The re-roll chooses randomly, so you *might* get the same weather twice. Oops!
    + To re-roll, type `weather reroll` in the terminal.

### Rotational Modifier Store

"As part of joining the program, we provided you with some new technology on your terminal! \
By otherwordly means, you can modify moon experiences to make things easier for you! \
Unfortunately, we couldn't lift the curse on them, so... they come with *slight* drawbacks."

**Easy Modifiers**
- Shiny but Swift: Spawnable scrap becomes 7% more valuable, but enemies move 7% faster.
- More Risk, More Reward: Increase spawnable scrap by 2, but add 2 indoor and outdoor power.
- Risk of Rain: Remove 2 indoor and outdoor power, but increase meteor shower event chance by 10%.
- Edmund's Moon: Time moves 15% slower, but decreases amount of scrap by 2.

**Medium Modifiers**
- Miller's Moon: Spawnable scrap becomes 20% more valuable, but time moves 25% faster.
- Mothron's Dawn: Spawnable scrap becomes 15% more valuable and spawn 3 more scrap, but weather becomes eclipsed and indoor/outdoor power increases by 2.
- Watch Your Back: Increase spawnable scrap by 5, but add 3 indoor power and only bracken's spawn.
- Shaped Glass: Spawn 1.5x the amount of scrap, but lose half your health.
- Lightning Speed: Gain 2x movement speed, but enemies gain 3x movement speed.

**Hard Modifiers**
- Go Play Outside! : Removes all indoor power of the moon, but adds it to outdoor power.
- Go Play Inside! : Removes all outdoor power of the moon, but adds it to indoor power.

You can access the store by typing `rot store` in the terminal.

## To-be Offered

More to come soon, such as:
- More modifiers
- Ship Upgrades
- And more...

## NewGame+ Functionality

Did your squad wipe and lose the run? \
Well don't fret, because obtained upgrades always stay with you in the same save file! 

# General Information

On a side note, welcome to the Lethal Upgrades mod! \
Trying to bring less stagnation and more gameplay diversity through this mod. \
Upgrades are usually focused on different categories that benefit the player. \
These upgrades either take credits or use tokens, such as the legendary ones! \
Upgrades are *progressive*, meaning they need to be bought in order, except legendary ones. 

As of May 6, 2026, I also added a new rotational modifiers store! \
This store allows players to pick modifiers that will change their experience when gathering scrap. \
They provide both an incentive and a drawback to keep things interesting! \
Currently there are 6 modifiers available: 3 easy, 2 medium, and 1 hard. \
I want to add more, but I wanted to release these for now so everyone can feel them out.

The store system works as follows:
- When a host loads into their save, modifiers are rolled.
- When rolling, the game will choose 3 easy, 2 medium and 1 hard modifier for players to choose from.
- Players can only activate modifiers in orbit, and they will be re-rolled once they go back into orbit.
- Once a modifier is active, players are not allowed to reroute to other moons!
- Any amount of modifiers can be activated, so choose wisely!

You can access the store by typing `rot store` in the terminal.

<details>
    <summary> <b>Token System [SPOILERS]</b> </summary>
    Tokens are acquired by cummulative performances on the moons you go to!<br>
    In other words, how fast or slow you get a token depends on your performance!<br>
    <br>
    The mod uses a "token meter" which starts at 0 and goes up to 100.<br>
    Depending on your performance grade, the meter fills up a certain amount.<br>
    Once you reach 100, you get a token to claim a legendary upgrade!<br>
    <br>
    The amount per grade is as follows:
    <ul>
        <li> <b> Rank S: 50 </b> </li>
        <li> <b> Rank A: 35 </b> </li>
        <li> <b> Rank B: 10 </b> </li>
        <li> <b> Rank C: 5 </b> </li>
        <li> <b> Rank F: -10 </b> </li>
    </ul>
</details>

## Terminal Commands and Extra Information

Command Table for Upgrades (Some may not be implemented yet)

| Category | Tier 1               | Tier 2               | Tier 3               | Legendary                | Info                    |
|----------|----------------------|----------------------|----------------------|--------------------------|-------------------------|
| Health   | `upgrade health 1`   | `upgrade health 2`   | `upgrade health 3`   | `upgrade token health`   | `upgrade health info`   |
| Stamina  | `upgrade stamina 1`  | `upgrade stamina 2`  | `upgrade stamina 3`  | `upgrade token stamina`  | `upgrade stamina info`  |
| Movement | `upgrade movement 1` | `upgrade movement 2` | `upgrade movement 3` | `upgrade token movement` | `upgrade movement info` |
| Utility  | `upgrade utility 1`  | `upgrade utility 2`  | `upgrade utility 3`  | `upgrade token utility`  | `upgrade utility info`  |

Cost Table for Upgrades
| Category | Tier 1 | Tier 2 | Tier 3 | Legendary |
|----------|--------|--------|--------|-----------|
| Health   | $200    | $300    | $400    | 1 token   |
| Stamina  | $300    | $400    | $500    | 1 token   |
| Movement | $250    | $350    | $400    | 1 token   |
| Utility  | $250    | $350    | $400    | 1 token   |

Extra Commands
| Command                 | Description                                   |
|-------------------------|-----------------------------------------------|
| `upgrade`               | Brief explanation of the mod.                 |
| `upgrade list`          | Shows all upgrades and which ones you already have.                 |
| `upgrade token`         | Brief explanation on upgrade tokens.          |
| `upgrade health info`   | Summarizes health upgrades and their costs.   |
| `upgrade stamina info`  | Summarizes stamina upgrades and their costs.  |
| `upgrade movement info` | Summarizes movement upgrades and their costs. |
| `upgrade utility info`  | Summarizes utility upgrades and their costs.  |
| `shovel explosion`  | Toggles shovel explosion effect on or off. Client-sided.  |
| `shovel jump`  | Toggles shovel jumping on or off. Client-sided.  |
| `weather reroll` | Re-roll the weather of the moon you orbit. |
| `rot store` | Show the rotational modifiers store. |
| `tier 1 mod` | Shows the available easy modifiers |
| `tier 2 mod` | Show the available medium modifiers. |
| `tier 3 mod` | Show the available hard modifier. |

## Multiplayer Status

MULTIPLAYER STATUS: 🟡
- 🔴 Not working
- 🟡 Testing
- 🟢 Operational

## Bugs and Recommendations

Please report any bugs, specially multiplayer ones, to try and patch them ASAP. \
If you wanna report a bug or recommend something: 
- Form: https://forms.gle/L3VPdXoyWeRz6P858
- None serious responses WILL be ignored.

Still a WIP, so bare with me.