   M S B A M L   `   `   `  ���� D  @Celery, Version=19.701.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Celery.ConvertersCelery.Converters  )    #Celery.Controls.CelerySliderControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6
Convertersclr-namespace:Celery.Converters     $��95��$	��289��  � 	Resources  /  )Celery.Converters.DoubleToStringConverter   DoubleToStringConverter/  )Celery.Converters.InverseBooleanConverter  InverseBooleanConverter��%   &	        &	          .��� .+��� -    ��Name$ 
drawSlider��$��27��$��
10,10,10,0q�$	��Top=� � SmallChange$ 1Q� � LargeChange$ 20Q� � Minimum$ 1Q� � Maximum$	 500Q� � Value �� *SliderValue+�� -    ��Name$ minvalTextBlock��$
��Left=�$��21��$��	10,42,0,0q�$
��Wrap=�$	��Top=�$��50������ *Minval+ �� 	Converter# [  �� -   $ maxvalTextBlock��$��Right=�$��20��$��0,43,9,0q�$
��Wrap=�$	��Top=�$��50��	 �� TextAlignment$	 Right=����� *Maxval+# [  �� -   $ outputviewTextBlock��$��22��$��
65,43,64,0q�$
��Wrap=�$	��Top=�$	 Center=�$
��Bold"����� *OutputviewValue+# [  	� $��Integer��$
��Left=�$��20��$��10,0,0,0q�$��Bottom=�$��61��
 d� 	IsChecked
 �� *	IsInteger+	� $��Number��$
��Left=�$��20��$��76,0,0,0q�$��Bottom=�$��67��
 �� *
IsNumber+�� $
��Left=�$��25��$��	148,0,0,0q�$��Bottom=�$��41��{��� *	IsInteger+# [ ���� *PrecisionCollection+ �� SelectedItem �� *SliderPrecision+ �� UpdateSourceTrigger$ PropertyChanged=��� $��20��$��
194,0,10,0q�$
��Wrap=�$��decimal places��$��Bottom=�