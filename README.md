# PlotLab
a plot library can be used in WPF project

## Catalog

[Start](#start)

[Mode](#mode)

[More Attributes](#more-attributes)

## Start
We can use this library in the following ways.
first of all,make sure you are in a wpf project

you can download and add this like a User Control Library.and also you can get it from nuget.

more information: 
https://www.nuget.org/packages/PlotLab/1.0.1

#### For Example
    xmlns:controler="clr-namespace:PlotLab;assembly=PlotLab"
#### and then you can add component like follow
    <controler:PlotChart x:Name="plot"/>
#### and you must add reference in your code file
    using System.Drawing;
    using PlotLab;
#### you can add data points as below:
      plot.Sequence = new Sequence(new List<SequenceEntity>() {
                new SequenceEntity(new List<float>() { 1,2,3,4,5,6,7}),
                new SequenceEntity(new List<float>() { 0,5,3,4,5,6,7},Pens.Brown,"example 1"),
                new SequenceEntity(new List<float>() { 0,3,3,4,5,2,1},Pens.Green,"example 2"),
            });
#### using 'add' is also allowed.but you must use update function to show in the screen.
#### if there is a '=' you used in Sequence,update function is no need to use.
    plot.Sequence.PlotChartPoints[0].Values.Add(1.6f);
    plot.Sequence.PlotChartPoints[0].Values.Add(3.4f);
    plot.Sequence.PlotChartPoints[0].Values.Add(4.4f);
    plot.UpdatePlot();
#### you will see a photo like this follow:
![Example](https://github.com/dongfangyier/PlotLab/blob/master/img/example1.png)


## Mode
#### here is two modes you can choose,default is REPAINT_PER_DATA
##### if REPAINT_PER_DATA Mode is selected,update function will redraw whole paint every time you use it.

##### but in NO_REPAINT Mode,update function will only redraw the new points.
##### Tips:Before you choose NO_REPAINT Mode,_MinValue and _MaxValue must be set to suitable values.and you'd better set PointNum.
            plot._MinValue = 0;
            plot._MaxValue = 10;
            plot.Sequence = new Sequence(new List<SequenceEntity>() {
            new SequenceEntity(new List<float>() { 1,2,3,4,5,6,7}),
            new SequenceEntity(new List<float>() { 0,5,3,4,5,6,7},Pens.Brown,"example 1"),
            new SequenceEntity(new List<float>() { 0,3,3,4,5,2,1},Pens.Green,"example 2"),
        },PlotLab.PaintMode.NO_REPAINT);
#### what's more you can clear the paints,when the screen is full of lines.
    plot.ClearData();


## More Attributes
| order | Attribute|function
|---|---|---|
| 0| Sequence | the data list you want to show,you can set data and colour here |
| 1| Title|the title of the plot|
| 2 | _MaxValue|the max value of the list|
| 3 | _MinValue |the min value of the list|
| 4 | _Width|reserve property,not completed|
| 5 | _Height|reserve property,not completed|

| order | name|function
|---|---|---|
| 0| UpdatePlot() | update paints |
| 1| ClearData() |remove all line in the paints.|
| 2| ClearDataByIndex() |remove the appoint line in the paints.index starts at 0.|

### more information of Sequence
| order | Attribute|describe
|---|---|---|
| 0| PlotChartPoints | a list of SequenceEntity |
| 1| Mode | controller to control whether to redraw |
### more information of SequenceEntity
| order | Attribute|describe
|---|---|---|
| 0| Values | list of data that youwant to show |
| 1| Title|the title of this curve|
| 2 | Color|the colour of the curve|
    
    
