function InputAutomation(element)
{
    //if element does not contains the "aut-hint" attribute, creates this
    if (typeof(element.getAttribute("aut-hint")) =="undefined")
        element.setAttribute("aut-hint", "");

    if (element.getAttribute("aut-hint") != "")
    {
        element.addEventListener("focus", function(){
            if (element.value == element.getAttribute("aut-hint"))
            {
                element.value = "";
                element.className = element.className.replace(/hinted/g, "");
            }
          
        });

        element.addEventListener("blur", function(){
            if (element.value == "")
            {
                element.value =element.getAttribute("aut-hint");
                element.className += " hinted";
            }
        
        });
    }


    if (element.value == "")
    {
        element.value =element.getAttribute("aut-hint");
        element.className += " hinted";
    }

}



//this class looks byt inputs, on html, with property automation="InputAutomation"
window.addEventListener("load", function(){
    inputs = document.querySelectorAll("input[automation=InputAutomation]");

    for (var cont = 0; cont < inputs.length; cont++)
    {
        new InputAutomation(inputs[cont]);
    }
});