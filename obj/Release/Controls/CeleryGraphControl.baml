   M S B A M L   `   `   `  ���� D  @Celery, Version=19.701.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Celery.ControlsCelery.Controls  5clr-namespace:Celery.ConvertersCelery.Converters  %clr-namespace:Celery.UI	Celery.UI  I)clr-namespace:Celery.Controls.SubcontrolsCelery.Controls.Subcontrols  (    "Celery.Controls.CeleryGraphControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     /localclr-namespace:Celery.Controls     0convclr-namespace:Celery.Converters     &UIclr-namespace:Celery.UI     ASubcontrols)clr-namespace:Celery.Controls.Subcontrols     -   $	��300��$	��300��  � 	Resources  +   %Celery.Converters.BorderClipConverter   BorderClipConv��%   &	           � DataContext   Celery.UI.GraphMap  .��� .+��� -    ��Name$ 
drawBorder��$��0,0,0,39q�
�������$	��1.5q� �� CornerRadius$ 1�� K� ZIndex$ 10000�� �� Padding$ 0q���
� .���  � Color$ 	#FF666666�� � Offset$ 0Q�� $ 	#FF9C9C9C��$ 1Q�._��� -    ��Name$ 
thisCanvas��$
��Auto��$
��Auto��}�. $��0q�.+��� -   	 ��Name$	 thisCanvasErrorText��$��30(�
���  ��$��ERROR!��
 �� Left$
 10��
 �� Top$	 101�� �� TextAlignment$ Center=�$	��274��$��45��$
��Bold"�$y�Hidden=�����  �� Color$ 	#FFFDFDFD�� �� Opacity$	 0.6Q��� -   $	 	textBlock��$
��Left=�$��16��$��	10,0,0,15q�$
��Wrap=�$��
Curve Type��$��Bottom=�$��73���� -    ��Name$ comboBox��$��24��$��
88,0,40,10q�$��Bottom=�����  �� Source *   $Celery.UI.EnumBindingSourceExtension  *M� *UI:CurveTypes++ �� SelectedItem �� *	CurveType+0   *Celery.Controls.Subcontrols.ceallPointGrip  0   *Celery.Controls.Subcontrols.ceallPointGrip   	DragDelta$ ceallPointGrip_DragDelta��$��Right=�$��15��$��Bottom=�$��15��