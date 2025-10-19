from langchain_ollama import OllamaLLM
from langchain_core.prompts import ChatPromptTemplate 

model = OllamaLLM(model="gemma3:4b")

template = """
make a commmit massage .

here are some relevant reviews: {reviews}

here is the question to answer: {question}
"""
prompt = ChatPromptTemplate.from_template(template)
chain = prompt | model

result = chain.invoke({"reviews": [], "question": "i add a code give me a univers sal massage"})
print(result)
