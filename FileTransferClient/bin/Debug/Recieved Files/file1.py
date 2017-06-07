def Max(list_):
    m_=0
    for i in range(list_length):
        if(i>m_):
            m_=i
    return m_


var1 = ""
var_list = []
while var1 != "done":
    var1 = float(input("enter element: "))
    var_list.insert(0, var1)
    var1 = input("done? ")

print(Max(var_list))
    
