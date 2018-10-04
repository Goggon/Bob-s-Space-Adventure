THIS ASSET REQUIRES A COPY OF ASEPRITE, YOU CAN FIND IT HERE: http://www.aseprite.org

Use:
>> From .ase file
1 - Choose an aseprite file to import (it must be in the assets folder)
2 - Select in the options if you would like to import a spritesheet, import animation (this won't be done if a spritesheet isn't imported) and create a prefab.
3 - Select if you want the animation to be played on a child of the gameobject with the animator (recommended)
4 - Select if you want to use tags to separate animations and if you want your animations to loop
5 - Click Export* to get everything you need
*Unity only allows to write text files if the platform is set to standalone, so if the platform is set to something different, the button will change to "Change to standalone" and it will do the change if you press it.

>> From spritesheet and atlas (.json) file
1 - If you don't want to use Aseprite importer to create the spritesheet and atlas file, you should use these command line arguments to export it
    If your animations have tags:
        -b "PATH/TO/FILE.ase" --filename-format '{frame000}_{tag}' --sheet-pack --sheet "PATH/TO/SPRITESHEET.png"
    If your animation doesn't have any tags:
        -b "PATH/TO/FILE.ase" --filename-format '{frame000}_{title}' --sheet-pack --sheet "PATH/TO/SPRITESHEET.png"

    If you want to add a border, you need the next before "--sheet":
        --inner-padding BORDER

    (It is important that the name of the frame starts with the frame number and that it has enough zeroes to the left to sort them as strings, otherwise your animations will get disordered)

2 - Open the foldout "Import from atlas"
3 - Put the spritesheet file in "Spritesheet"
4 - Put the json atlas in "Atlas"
5 - Go though steps 2 to 4 in the "From .ase file" instructions
6 - Press Import to get everything you need


For support you can mail me at rhosupport@polyraptor.com