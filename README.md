# PlotLab
a plot library can be used in WPF project

## Catalog

[Start](#start)

[More Attributes](#more-attributes)

## Start
We can use this library in the following ways.
first of all,make sure you are in a wpf project

you can add this like a User Control.
#### For Example
    xmlns:controler="clr-namespace:PlotLab"
#### and then you can use like follow
    <controler:PlotChart x:Name="plot"/>
#### you can add point of data like follow:
      plot.Sequence = new Sequence(new List<SequenceEntity>() {
                new SequenceEntity(new List<float>() { 1,2,3,4,5,6,7}),
                new SequenceEntity(new List<float>() { 0,5,3,4,5,6,7},Pens.Brown,"example 1"),
                new SequenceEntity(new List<float>() { 0,3,3,4,5,2,1},Pens.Green,"example 2"),
            });
#### using 'add' is also allowed.
    plot.Sequence.PlotChartPoints[0].Values.Add(1.6f);
#### you will see a photo like this follow:
![Example](https://github.com/dongfangyier/PlotLab/blob/master/img/example1.png)

## More Attributes
| order | Attribute|function
|---|---|---|
| 0| Sequence | the data list you want to show,you can set data and colour here |
| 1| Title|the title of the plot|
| 2 | _MaxValue|the max value of the list|
| 3 | _MinValue |the min value of the list|
| 4 | _Width|reserve property,not completed|
| 5 | _Height|reserve property,not completed|
### more information of Sequence
| order | Attribute|function
|---|---|---|
| 0| PlotChartPoints | the data list you want to show,it contains a list of float and a list of colour and title of the list |

    
    
