# PS6 - Spreadsheet GUI

Luke Ludlow

CS 3500

2019 October

---

# design


the basic project structure and flow is this:

(note: arrow A -> B means project A references project B)

SpreadsheetGUI(View) -> SpreadsheetGUIController -> SpreadsheetGUIModel


the SpreadsheetGUIEvents is the "glue" that loosely connects the view and controller:

SpreadsheetGUI(View) -> SpreadsheetGUIEvents

SpreadsheetGUIController -> SpreadsheetGUIEvents


my spreadsheet gui follows the model view controller design pattern.

the SpreadsheetGUI project is the view. 
most importantly, it's the package that handles all the windows forms dependencies and such. 
all the other projects are just standard c# class library projects.
the SpreadsheetGUIView object is the windows form generated object, i just renamed it.
it has access to a controller, and that's it.

the SpreadsheetGUIController project doesn't actually reference the SpreadsheetGUIView. this is because
it would cause a circular dependency (visual studio will literally throw an error when i try to 
reference the view project from the controller). 

instead, the controller communicates with the view by emitting events. this provides a clean separation 
of concerns between the controller and view. the view class methods essentially just call the corresponding 
controller method, and then the controller emits events when it needs to do anything to the view. 
furthermore, the controller doesn't know what happens to the view, it just invokes the event, which is good practice.

the SpreadsheetGUIModel project is essentially a wrapper over the Spreadsheet class, with some additional functionality 
added as needed by the controller. 

the SpreadsheetGUIEvents is the "glue" that loosely connects the view and controller. it is a collection of
event type delegates that the view and controller both reference. this allows a loose contract to exist so
that the controller can send commands to the view.



# additional creative feature

my spreadsheet gui has a dark mode! click the sun/moon button on the top right to toggle dark mode.
the spreadsheet gui starts off in light mode. 

to implement this feature i had to clone the spreadsheet panel component class and add my own methods.

the SpreadsheetGUIView, SpreadsheetPanel, and the SpreadsheetPanel.DrawingPanel all have the following two methods:
`ActivateLightMode()` and `ActivateDarkMode()`.

when the controller emits the event to tell the view to activate light or dark mode, the SpreadsheetGUIView 
changes the background and foreground color of all the components that it controls.
then it calls activate light/dark mode on the spreadsheet panel. the spreadsheet panel does the same,
where it changes the colors of the components that it controls, and then calls activate light/dark mode on
its internal drawing panel. 

informally, the flow would look like this:

click light/dark mode button event -> controller toggle dark mode (figures out which mode to activate) 
-> SpreadsheetGUIView activate light/dark mode -> SpreadsheetPanel activate light/dark mode -> 
DrawingPanel activate light/dark mode
