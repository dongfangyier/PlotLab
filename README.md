# PlotLab
## 一个为C# WPF框架设计的画图组件。通过简单的引用即可使用，该组件最新版本支持绑定。

A drawing library for WPF framework.

## Catalog

[Start](#start)

[Mode](#mode)

[Binding](#binding)

[More Properties](#more-properties)

## Start
We can use the library as follows.
First, make sure you are using the WPF framework.

You can download and add this like an User-Control, and you can also download it at nuget.

more information: 
https://www.nuget.org/packages/PlotLab/1.0.3

#### For Example:
    xmlns:controler="clr-namespace:PlotLab;assembly=PlotLab"
#### And then you can add component as follow:
    <controler:PlotChart x:Name="plot"/>
#### Add reference in script file:
    using System.Drawing;
    using PlotLab;
#### Add data points in the following way:
     plot._Sequence = new Sequence(new ObservableCollection<SequenceEntity>() {
            new SequenceEntity(new ObservableCollection<float>() { 1,2,3,4,5,6,7}),
            new SequenceEntity(new ObservableCollection<float>() { 0,5,3,4,5,6,7},Pens.Brown,"example 1"),
            new SequenceEntity(new ObservableCollection<float>() { 0,3,3,4,5,2,1},Pens.Green,"example 2"),
        });
#### Using 'add' function is also allowed. However, when using the add function, you must use the update function to display on the screen.
    plot._Sequence.PlotChartPoints[0].Values.Add(3.4f);
    plot._Sequence.PlotChartPoints[0].Values.Add(4.4f);
    plot.UpdatePlot();
#### You can remove a complete broken line from graph. Besides, 'UpdatePlot()' is needed to display on screen.
    plot.ClearDataByIndex(0);
    plot.UpdatePlot();
#### You can remove all broken line from graph. Besides, 'UpdatePlot()' is needed, too.
    plot.ClearData();
    plot.UpdatePlot();
#### You will see a photo like this follow:
![Example](https://github.com/dongfangyier/PlotLab/blob/master/img/example1.png)


## Mode
#### Here is two modes you can choose, default is REPAINT_PER_DATA.
##### If REPAINT_PER_DATA Mode is selected, the update function redraws the entire drawing each time it is used.

##### But in NO_REPAINT Mode, update function will only draw the new points.
##### Tips:Before you choose NO_REPAINT Mode, _MinValue and _MaxValue must be set to appropriate values. And you'd better set PointNum.
            plot._MinValue = 0;
            plot._MaxValue = 10;
            plot._Sequence = new Sequence(new ObservableCollection<SequenceEntity>() {
            new SequenceEntity(new ObservableCollection<float>() { 1,2,3,4,5,6,7}),
            new SequenceEntity(new ObservableCollection<float>() { 0,5,3,4,5,6,7},Pens.Brown,"example 1"),
            new SequenceEntity(new ObservableCollection<float>() { 0,3,3,4,5,2,1},Pens.Green,"example 2"),
        },PlotLab.PaintMode.NO_REPAINT);

## Binding
#### In the version 1.0.3 and beyond, you can use binding like most of controls. 
    <controler:PlotChart
            x:Name="plot"
            _UpdatePlot="{Binding Path=_UpdatePlot,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}"
            _ClearData="{Binding Path=ClearData,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}"
            _ClearDataByIndex="{Binding Path=ClearDataByIndex,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}"
            _MinValue="{Binding Path=_MinValue,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}"
            _MaxValue="{Binding Path=_MaxValue,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}"
            _Sequence="{Binding Path=_Sequence,Mode=TwoWay,UpdateSourceTrigger=PropertyChanged}" />
#### For convenience，when you using binding,you can set _UpdatePlot, _ClearDataByIndex and _ClearData to repleace these function include UpdatePlot(),ClearDataByIndex(index) and ClearData().
    // add a data point
     _Sequence.PlotChartPoints[0].Values.Add(1.2f);
     _UpdatePlot = true;
     NotifyPropertyChanged("_UpdatePlot");
    
    
    // clear one broken line
     ClearDataByIndex = 0;
     NotifyPropertyChanged("ClearDataByIndex");
     _UpdatePlot = true;
     NotifyPropertyChanged("_UpdatePlot");
   
   
    // clear all broken line
     ClearData = true;
     NotifyPropertyChanged("ClearData");
     _UpdatePlot = true;
     NotifyPropertyChanged("_UpdatePlot");




## More Properties
| order | Properties|function
|---|---|---|
| 0| _Sequence | the data ObservableCollection you want to show,you can set data and colour here |
| 1| Title|the title of the plot|
| 2 | _MaxValue|the max value of the list|
| 3 | _MinValue |the min value of the list|
| 4 | _Width|reserve property,not completed|
| 5 | _Height|reserve property,not completed|
| 6 | _UpdatePlot |display your changes to the screen|
| 7 | _ClearDataByIndex|clear one broken line in the graph|
| 8 | _ClearData|clear all broken line in the graph|

| order | name|function
|---|---|---|
| 0| UpdatePlot() | update paints |
| 1| ClearData() |remove all line in the paints.|
| 2| ClearDataByIndex() |remove the appoint line in the paints.index starts at 0.|

### more information of Sequence
| order | Properties|describe
|---|---|---|
| 0| PlotChartPoints | a ObservableCollection of SequenceEntity |
| 1| Mode | controller to control whether to redraw |
### more information of SequenceEntity
| order | Properties|describe
|---|---|---|
| 0| Values | ObservableCollection of data that you want to show |
| 1| Title|the title of this curve|
| 2 | Color|the colour of the curve|
    
    
