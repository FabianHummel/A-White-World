# Todo-List:

## Engine Structure:

The engine contains the game scene + a persistent engine-specific scene that is never unloaded.
These scenes run in parallel and are executed in a fixed order. First the game-scene, then the engine scene.


### Game Scene



### Engine Scene

The engine scene loads resources that are present permanently during runtime.
This includes the dialogue's sound effects and all different fonts.
The following categories are implemented as gameobjects inside the engine scene.
They can be referred to from anywhere within the code, because they are persistent.

#### Dialogue

Use custom UI system for complex nested layouts.

#### Curtain

Solve using GLSL Shader that draws to a rendertexture.
This texture is then displayed here.

#### Pause-Menu

Hardcoded pause menu, may not even use the ui-system.

## User Interface System:

When drawing one the things listed below, a content node may optionally be accepted as a parameter.
Everything inside the content is relative to the parent.
Each of these categories are nodes that may be plugged into another node.
The align controls the position inside a parent. The position is a relative offset to that.

* Rectangle (x, y, w, h, color, alignX, alignY, content?)
* Image (image, x, y, alignX, alignY, content?)
* Stack (x, y, w, h, params content?)
* etc...

Note the stack node, it accepts multiple nodes.
This is useful for when two or more sepearate nodes need to sit next to each other.

Maybe I will use RayGui for that, lets see...
