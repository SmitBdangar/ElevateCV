import openai
from typing import List
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity
import os
from dotenv import load_dotenv

load_dotenv()

class EmbeddingMatcher:
    
    def __init__(self, api_key: str = None):
        self.api_key = api_key or os.getenv('OPENAI_API_KEY')
        if self.api_key:
            openai.api_key = self.api_key
    
    def get_embedding(self, text: str, model: str = "text-embedding-3-small") -> List[float]:
        if not self.api_key:
            # Return dummy embedding for demo
            return np.random.rand(1536).tolist()
        
        try:
            # Truncate text to avoid token limits
            text = text[:8000]
            
            response = openai.embeddings.create(
                input=text,
                model=model
            )
            return response.data[0].embedding
        except Exception as e:
            print(f"Embedding error: {e}")
            return np.random.rand(1536).tolist()
    
    def calculate_similarity(self, text1: str, text2: str) -> float:
        emb1 = self.get_embedding(text1)
        emb2 = self.get_embedding(text2)
        
        similarity = cosine_similarity([emb1], [emb2])[0][0]
        return float(similarity)